using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Settings;

namespace API.Auth;

public sealed class TwoFactorAuthEmailProcessor
{
    private readonly UserManager<ApplicationUser> _usersManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AuthTokenGenerator _authTokenGenerator;
    private readonly IOptionsMonitor<EmailSettings> _emailSettings;
    private readonly IOptionsMonitor<AuthSettings> _authSettings;
    private readonly ILogger<TwoFactorAuthEmailProcessor> _logger;

    public TwoFactorAuthEmailProcessor(SignInManager<ApplicationUser> signInManager, ILogger<TwoFactorAuthEmailProcessor> logger, UserManager<ApplicationUser> usersManager, AuthTokenGenerator authTokenGenerator, IOptionsMonitor<EmailSettings> emailSettings, IOptionsMonitor<AuthSettings> authSettings)
    {
        _signInManager = signInManager;
        _logger = logger;
        _usersManager = usersManager;
        _authTokenGenerator = authTokenGenerator;
        _emailSettings = emailSettings;
        _authSettings = authSettings;
    }

    // ✅ УНИВЕРСАЛЬНЫЙ МЕТОД ДЛЯ ВСЕХ ВНЕШНИХ ПРОВАЙДЕРОВ
    public async Task<TokenResponse> ProcessExternalLoginAsync(
        string provider,
        string providerKey,
        string email,
        string? name = null,
        string? phoneNumber = null)
    {
        try
        {
            // 1. Пытаемся войти через существующий внешний логин
            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(
                loginProvider: provider,
                providerKey: providerKey,
                isPersistent: true);

            _logger.LogInformation("ExternalLoginSignInAsync result for {Provider}: {Result}", provider, signInResult);

            // 2. Обработка успешного входа
            if (signInResult.Succeeded)
            {
                ApplicationUser? user = await _usersManager.FindByLoginAsync(provider, providerKey);
                if (user != null)
                {
                    string accessToken = await _authTokenGenerator.GenerateAccessToken(user);
                    string refreshToken = await _authTokenGenerator.GenerateRefreshToken(user);
                    _ = await _usersManager.SetAuthenticationTokenAsync(user, "SQLServer", "AccessToken", accessToken);
                    _ = await _usersManager.SetAuthenticationTokenAsync(user, "SQLServer", "RefreshToken", refreshToken);
                    return new TokenResponse(true, accessToken, _authSettings.CurrentValue.AccessTokenLifetimeMinutes, null);
                }
                return new TokenResponse(false, null, 0, "user_not_found");
            }

            // 3. Обработка 2FA
            if (signInResult.RequiresTwoFactor)
            {
                ApplicationUser? user = await _usersManager.FindByLoginAsync(provider, providerKey);
                if (user != null)
                {
                    string twoFactorToken = await _usersManager.GenerateTwoFactorTokenAsync(user, "Email");
                    await SendTwoFactorEmailAsync(user.Email, twoFactorToken);

                    return new TokenResponse(false, null, 0, "requires_two_factor");
                }
                return new TokenResponse(false, null, 0, "user_not_found_for_2fa");
            }

            // 4. Обработка блокировки
            if (signInResult.IsLockedOut)
                return new TokenResponse(false, null, 0, "account_locked");

            // 5. Пользователь не найден - ищем по email или создаем нового
            return await CreateOrLinkUserAsync(provider, providerKey, email, name, phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing external login for {Provider}", provider);
            return new TokenResponse(false, null, 0, $"processing_error: {ex.Message}");
        }
    }

    // ✅ МЕТОД ДЛЯ СОЗДАНИЯ ИЛИ ПРИВЯЗКИ ПОЛЬЗОВАТЕЛЯ
    private async Task<TokenResponse> CreateOrLinkUserAsync(
        string provider,
        string providerKey,
        string email,
        string? name = null,
        string? phoneNumber = null)
    {
        // Проверяем, существует ли пользователь с таким email
        ApplicationUser? existingUser = await _usersManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            // Пользователь существует - привязываем внешний аккаунт
            _logger.LogInformation("Linking {Provider} account to existing user {Email}", provider, email);

            // Проверяем, не привязан ли уже этот внешний аккаунт к кому-то другому
            ApplicationUser? existingLogin = await _usersManager.FindByLoginAsync(provider, providerKey);
            if (existingLogin != null)
                return new TokenResponse(false, null, 0, "external_account_already_linked");

            IdentityResult addLoginResult = await _usersManager.AddLoginAsync(existingUser,
                new UserLoginInfo(provider, providerKey, provider));

            if (addLoginResult.Succeeded)
            {
                string geteratedToken = await _authTokenGenerator.GenerateAccessToken(existingUser);
                _ = await _usersManager.SetAuthenticationTokenAsync(existingUser, "SQLServer", "AccessToken", geteratedToken);
                return new TokenResponse(true, geteratedToken, _authSettings.CurrentValue.AccessTokenLifetimeMinutes, null);
            }

            return new TokenResponse(false, null, 0, $"link_failed: {string.Join(", ", addLoginResult.Errors.Select(e => e.Description))}");
        }

        // Создаем нового пользователя
        _logger.LogInformation("Creating new user for {Provider} login with email {Email}", provider, email);

        ApplicationUser newUser = new()
        {
            Email = email,
            UserName = GenerateUniqueUsername(name ?? email),
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = (name ?? email).ToUpperInvariant(),
            EmailConfirmed = true,
            PhoneNumber = phoneNumber,
            PhoneNumberConfirmed = !string.IsNullOrEmpty(phoneNumber),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString(),
            TwoFactorEnabled = false,
            LockoutEnabled = true
        };

        IdentityResult createResult = await _usersManager.CreateAsync(newUser);

        if (!createResult.Succeeded)
            return new TokenResponse(false, null, 0, $"user_creation_failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

        // Получаем созданного пользователя
        ApplicationUser? createdUser = await _usersManager.FindByEmailAsync(email);
        if (createdUser == null)
            return new TokenResponse(false, null, 0, "user_not_found_after_creation");

        // Добавляем внешний логин
        IdentityResult addLoginResultForNew = await _usersManager.AddLoginAsync(createdUser,
            new UserLoginInfo(provider, providerKey, provider));

        if (!addLoginResultForNew.Succeeded)
        {
            // Откатываем создание пользователя
            _ = await _usersManager.DeleteAsync(createdUser);
            return new TokenResponse(false, null, 0, $"external_login_add_failed: {string.Join(", ", addLoginResultForNew.Errors.Select(e => e.Description))}");
        }

        // Генерируем токен
        string token = await _authTokenGenerator.GenerateAccessToken(createdUser);
        _ = await _usersManager.SetAuthenticationTokenAsync(createdUser, "SQLServer", "AccessToken", token);

        return new TokenResponse(true, token, 0, null);
    }

    // ✅ ВСПОМОГАТЕЛЬНЫЙ МЕТОД ДЛЯ ГЕНЕРАЦИИ УНИКАЛЬНОГО USERNAME
    private string GenerateUniqueUsername(string baseUsername)
    {
        // Очищаем username от пробелов и спецсимволов
        string cleanUsername = System.Text.RegularExpressions.Regex.Replace(baseUsername, @"[^a-zA-Z0-9_]", "");

        if (string.IsNullOrEmpty(cleanUsername))
            cleanUsername = $"user_{DateTime.Now.Ticks}";

        // Проверяем уникальность
        ApplicationUser? existingUser = _usersManager.FindByNameAsync(cleanUsername).Result;
        if (existingUser != null)
            return $"{cleanUsername}_{DateTime.Now.Ticks}";

        return cleanUsername.ToLowerInvariant();
    }

    // ✅ ВСПОМОГАТЕЛЬНЫЙ МЕТОД ДЛЯ ОТПРАВКИ 2FA EMAIL
    private async Task SendTwoFactorEmailAsync(string email, string token)
    {
        try
        {
            MimeMessage emailMessage = new();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.CurrentValue.Sender.Name, _emailSettings.CurrentValue.Sender.Email));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Confirm login";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"Your 2FA verification code is: <strong>{token}</strong><br><br>" +
                       $"Enter this code to complete your login."
            };

            using SmtpClient client = new();
            await client.ConnectAsync(_emailSettings.CurrentValue.Host, _emailSettings.CurrentValue.Port, _emailSettings.CurrentValue.UseSsl);
            await client.AuthenticateAsync(_emailSettings.CurrentValue.UserName, _emailSettings.CurrentValue.Password);
            _ = await client.SendAsync(emailMessage);
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
