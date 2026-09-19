using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using IdentityLibrary.Repositories.Tokens.RefreshTokens.Interfaces;
using IdentityLibrary.Services.Classes;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using Settings;

namespace API.Auth;

public sealed class TwoFactorAuthEmailProcessor
{
    private readonly UserManager<ApplicationUser> _usersManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AuthTokenGenerator _authTokenGenerator;
    private readonly IRefreshTokensRepository _refreshTokensRepo;
    private readonly IOptionsMonitor<EmailSettings> _emailSettings;
    private readonly IOptionsMonitor<AuthSettings> _authSettings;
    private readonly ILogger<TwoFactorAuthEmailProcessor> _logger;

    public TwoFactorAuthEmailProcessor(
        SignInManager<ApplicationUser> signInManager,
        ILogger<TwoFactorAuthEmailProcessor> logger,
        UserManager<ApplicationUser> usersManager,
        AuthTokenGenerator authTokenGenerator,
        IRefreshTokensRepository refreshTokensRepo,
        IOptionsMonitor<EmailSettings> emailSettings,
        IOptionsMonitor<AuthSettings> authSettings)
    {
        _signInManager = signInManager;
        _logger = logger;
        _usersManager = usersManager;
        _authTokenGenerator = authTokenGenerator;
        _refreshTokensRepo = refreshTokensRepo;
        _emailSettings = emailSettings;
        _authSettings = authSettings;
    }

    /// <summary>
    /// Обрабатывает внешний логин (OAuth). Возвращает AuthResponseDto с токенами
    /// или флагом RequiresTwoFactor.
    /// </summary>
    /// <param name="provider">Имя провайдера ("VK ID", "Google", ...)</param>
    /// <param name="providerKey">Стабильный идентификатор пользователя у провайдера (VK user_id)</param>
    /// <param name="email">Email от провайдера (может быть null)</param>
    /// <param name="name">Полное имя (FirstName + LastName)</param>
    /// <param name="phoneNumber">Телефон (может быть null)</param>
    public async Task<AuthResponseDto> ProcessExternalLoginAsync(
        string provider,
        string providerKey,
        string? email = null,
        string? name = null,
        string? phoneNumber = null)
    {
        try
        {
            // 1. Пытаемся найти пользователя по external login
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                loginProvider: provider,
                providerKey: providerKey,
                isPersistent: true);

            _logger.LogInformation(
                "ExternalLoginSignInAsync for {Provider}: {Result}",
                provider, signInResult);

            // 2. Пользователь уже привязан — логиним
            if (signInResult.Succeeded)
            {
                var user = await _usersManager.FindByLoginAsync(provider, providerKey);
                if (user is null)
                {
                    _logger.LogWarning("User not found after successful sign-in for {Provider}", provider);
                    return new AuthResponseDto(false, false, "user_not_found", string.Empty, string.Empty);
                }

                return await IssueTokensAsync(user);
            }

            // 3. Требуется 2FA
            if (signInResult.RequiresTwoFactor)
            {
                var user = await _usersManager.FindByLoginAsync(provider, providerKey);
                if (user is null)
                {
                    _logger.LogWarning("User not found for 2FA for {Provider}", provider);
                    return new AuthResponseDto(false, false, "user_not_found_for_2fa", string.Empty, string.Empty);
                }

                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    _logger.LogWarning("2FA required but user {UserId} has no email", user.Id);
                    return new AuthResponseDto(false, false, "no_email_for_2fa", string.Empty, string.Empty);
                }

                string twoFactorToken = await _usersManager.GenerateTwoFactorTokenAsync(user, "Email");
                await SendTwoFactorEmailAsync(user.Email, twoFactorToken);

                return new AuthResponseDto(false, true, "requires_two_factor", string.Empty, string.Empty);
            }

            // 4. Аккаунт заблокирован
            if (signInResult.IsLockedOut)
            {
                _logger.LogWarning("Account locked out for {Provider}", provider);
                return new AuthResponseDto(false, false, "account_locked", string.Empty, string.Empty);
            }

            // 5. Пользователя нет — создаём или привязываем к существующему
            return await CreateOrLinkUserAsync(provider, providerKey, email, name, phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing external login for {Provider}", provider);
            return new AuthResponseDto(false, false, $"processing_error: {ex.Message}", string.Empty, string.Empty);
        }
    }

    /// <summary>
    /// Создаёт нового пользователя или привязывает external login к существующему.
    /// </summary>
    private async Task<AuthResponseDto> CreateOrLinkUserAsync(
        string provider,
        string providerKey,
        string? email,
        string? name,
        string? phoneNumber)
    {
        // 1. Если email указан — ищем пользователя с таким email
        ApplicationUser? existingUser = null;
        if (!string.IsNullOrWhiteSpace(email))
        {
            existingUser = await _usersManager.FindByEmailAsync(email);
        }

        // 2. Пользователь с таким email уже существует — привязываем external login
        if (existingUser is not null)
        {
            _logger.LogInformation(
                "Linking {Provider} account to existing user {Email}",
                provider, email);

            var existingLogin = await _usersManager.FindByLoginAsync(provider, providerKey);
            if (existingLogin is not null)
            {
                _logger.LogWarning("External account already linked for {Provider}", provider);
                return new AuthResponseDto(false, false, "external_account_already_linked", string.Empty, string.Empty);
            }

            var addLoginResult = await _usersManager.AddLoginAsync(
                existingUser,
                new UserLoginInfo(provider, providerKey, provider));

            if (!addLoginResult.Succeeded)
            {
                var errors = string.Join(", ", addLoginResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to link external login: {Errors}", errors);
                return new AuthResponseDto(false, false, $"link_failed: {errors}", string.Empty, string.Empty);
            }

            // Обновляем недостающие данные
            bool needsUpdate = false;

            if (string.IsNullOrWhiteSpace(existingUser.PhoneNumber) && !string.IsNullOrWhiteSpace(phoneNumber))
            {
                existingUser.PhoneNumber = phoneNumber;
                existingUser.PhoneNumberConfirmed = false;
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                await _usersManager.UpdateAsync(existingUser);
            }

            return await IssueTokensAsync(existingUser);
        }

        // 3. Создаём нового пользователя
        _logger.LogInformation(
            "Creating new user for {Provider} (email: {Email}, providerKey: {ProviderKey})",
            provider, email ?? "<none>", providerKey);

        string usernameBase = !string.IsNullOrWhiteSpace(email)
            ? email
            : !string.IsNullOrWhiteSpace(name)
                ? name
                : $"user_{providerKey}";

        string uniqueUsername = await GenerateUniqueUsernameAsync(usernameBase);

        bool hasEmail = !string.IsNullOrWhiteSpace(email);

        var newUser = new ApplicationUser
        {
            // Email может быть null — это нормально для OAuth без email
            Email = hasEmail ? email : null,
            UserName = uniqueUsername,
            NormalizedEmail = hasEmail ? email!.ToUpperInvariant() : null,
            NormalizedUserName = uniqueUsername.ToUpperInvariant(),
            // Email НЕ подтверждён, даже если он есть — OAuth не подтверждает владение
            EmailConfirmed = false,
            PhoneNumber = phoneNumber,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var createResult = await _usersManager.CreateAsync(newUser);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            _logger.LogError("Failed to create user: {Errors}", errors);
            return new AuthResponseDto(false, false, $"user_creation_failed: {errors}", string.Empty, string.Empty);
        }

        // 4. Привязываем external login
        var addLoginResultForNew = await _usersManager.AddLoginAsync(
            newUser,
            new UserLoginInfo(provider, providerKey, provider));

        if (!addLoginResultForNew.Succeeded)
        {
            var errors = string.Join(", ", addLoginResultForNew.Errors.Select(e => e.Description));
            _logger.LogError("Failed to add external login, rolling back user creation: {Errors}", errors);

            // Откатываем создание пользователя
            await _usersManager.DeleteAsync(newUser);

            return new AuthResponseDto(false, false, $"external_login_add_failed: {errors}", string.Empty, string.Empty);
        }

        return await IssueTokensAsync(newUser);
    }

    /// <summary>
    /// Генерирует пару access/refresh токенов для пользователя.
    /// </summary>
    private async Task<AuthResponseDto> IssueTokensAsync(ApplicationUser user)
    {
        // Отзываем все старые refresh tokens пользователя
        await _refreshTokensRepo.RevokeAllByUserIdAsync(Convert.ToInt64(user.Id));

        string accessToken = await _authTokenGenerator.GenerateAccessToken(user);
        string refreshTokenValue = _authTokenGenerator.GenerateRefreshToken();

        var refreshToken = new RefreshToken(
            Id: 0,
            UserId: Convert.ToInt64(user.Id),
            Value: refreshTokenValue,
            IsRevoked: false,
            CreatedAt: DateTime.UtcNow);

        await _refreshTokensRepo.CreateAsync(refreshToken);

        return new AuthResponseDto(
            IsAuthSuccessful: true,
            RequiredTwoFactor: false,
            ErrorMessage: string.Empty,
            AccessToken: accessToken,
            RefreshToken: refreshTokenValue);
    }

    /// <summary>
    /// Генерирует уникальный username на основе базовой строки.
    /// </summary>
    private async Task<string> GenerateUniqueUsernameAsync(string baseUsername)
    {
        // Очищаем от недопустимых символов
        string cleanUsername = System.Text.RegularExpressions.Regex
            .Replace(baseUsername ?? string.Empty, @"[^a-zA-Z0-9_]", "");

        if (string.IsNullOrEmpty(cleanUsername))
        {
            cleanUsername = $"user_{DateTime.UtcNow.Ticks}";
        }

        // Ограничиваем длину (Identity обычно имеет лимит)
        if (cleanUsername.Length > 32)
        {
            cleanUsername = cleanUsername.Substring(0, 32);
        }

        // Проверяем уникальность
        var existingUser = await _usersManager.FindByNameAsync(cleanUsername);
        if (existingUser is not null)
        {
            // Добавляем timestamp для уникальности
            string suffix = $"_{DateTime.UtcNow.Ticks}";
            int maxBaseLength = 32 - suffix.Length;
            if (cleanUsername.Length > maxBaseLength)
            {
                cleanUsername = cleanUsername.Substring(0, maxBaseLength);
            }
            cleanUsername = $"{cleanUsername}{suffix}";
        }

        return cleanUsername.ToLowerInvariant();
    }

    /// <summary>
    /// Отправляет код 2FA на email.
    /// </summary>
    private async Task SendTwoFactorEmailAsync(string email, string token)
    {
        try
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(
                _emailSettings.CurrentValue.Sender.Name,
                _emailSettings.CurrentValue.Sender.Email));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Confirm login";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"Your 2FA verification code is: <strong>{token}</strong><br><br>" +
                       $"Enter this code to complete your login."
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _emailSettings.CurrentValue.Host,
                _emailSettings.CurrentValue.Port,
                _emailSettings.CurrentValue.UseSsl);
            await client.AuthenticateAsync(
                _emailSettings.CurrentValue.UserName,
                _emailSettings.CurrentValue.Password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);

            _logger.LogInformation("2FA email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send 2FA email to {Email}", email);
            throw;
        }
    }
}