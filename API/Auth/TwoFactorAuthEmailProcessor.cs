using API.Auth;
using IdentityLibrary.DTOs;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Settings;

namespace IdentityLibrary.Services;

public sealed class TwoFactorAuthEmailProcessor
{
    private readonly UserManager<ApplicationUser> _usersManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AuthTokenGenerator _authTokenGenerator;
    private readonly IOptionsMonitor<EmailSettings> _emailSettings;
    private readonly ILogger<TwoFactorAuthEmailProcessor> _logger;

    public TwoFactorAuthEmailProcessor(SignInManager<ApplicationUser> signInManager, ILogger<TwoFactorAuthEmailProcessor> logger, UserManager<ApplicationUser> usersManager, AuthTokenGenerator authTokenGenerator, IOptionsMonitor<EmailSettings> emailSettings)
    {
        _signInManager = signInManager;
        _logger = logger;
        _usersManager = usersManager;
        _authTokenGenerator = authTokenGenerator;
        _emailSettings = emailSettings;
    }

    // ✅ УНИВЕРСАЛЬНЫЙ МЕТОД ДЛЯ ВСЕХ ВНЕШНИХ ПРОВАЙДЕРОВ
    public async Task<(bool Success, string? Token, string? Error)> ProcessExternalLoginAsync(
        string provider,
        string providerKey,
        string email,
        string? name = null,
        string? phoneNumber = null)
    {
        try
        {
            // 1. Пытаемся войти через существующий внешний логин
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                loginProvider: provider,
                providerKey: providerKey,
                isPersistent: true);

            _logger.LogInformation("ExternalLoginSignInAsync result for {Provider}: {Result}", provider, signInResult);

            // 2. Обработка успешного входа
            if (signInResult.Succeeded)
            {
                var user = await _usersManager.FindByLoginAsync(provider, providerKey);
                if (user != null)
                {
                    var token = await _authTokenGenerator.GenerateJwtToken(user);
                    await _usersManager.SetAuthenticationTokenAsync(user, "SQLServer", "AuthToken", token);
                    return (true, token, null);
                }
                return (false, null, "user_not_found");
            }

            // 3. Обработка 2FA
            if (signInResult.RequiresTwoFactor)
            {
                var user = await _usersManager.FindByLoginAsync(provider, providerKey);
                if (user != null)
                {
                    var twoFactorToken = await _usersManager.GenerateTwoFactorTokenAsync(user, "Email");
                    await SendTwoFactorEmailAsync(user.Email, twoFactorToken);

                    return (false, null, "requires_two_factor");
                }
                return (false, null, "user_not_found_for_2fa");
            }

            // 4. Обработка блокировки
            if (signInResult.IsLockedOut)
            {
                return (false, null, "account_locked");
            }

            // 5. Пользователь не найден - ищем по email или создаем нового
            return await CreateOrLinkUserAsync(provider, providerKey, email, name, phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing external login for {Provider}", provider);
            return (false, null, $"processing_error: {ex.Message}");
        }
    }

    // ✅ МЕТОД ДЛЯ СОЗДАНИЯ ИЛИ ПРИВЯЗКИ ПОЛЬЗОВАТЕЛЯ
    private async Task<(bool Success, string? Token, string? Error)> CreateOrLinkUserAsync(
        string provider,
        string providerKey,
        string email,
        string? name = null,
        string? phoneNumber = null)
    {
        // Проверяем, существует ли пользователь с таким email
        var existingUser = await _usersManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            // Пользователь существует - привязываем внешний аккаунт
            _logger.LogInformation("Linking {Provider} account to existing user {Email}", provider, email);

            // Проверяем, не привязан ли уже этот внешний аккаунт к кому-то другому
            var existingLogin = await _usersManager.FindByLoginAsync(provider, providerKey);
            if (existingLogin != null)
            {
                return (false, null, "external_account_already_linked");
            }

            var addLoginResult = await _usersManager.AddLoginAsync(existingUser,
                new UserLoginInfo(provider, providerKey, provider));

            if (addLoginResult.Succeeded)
            {
                var geteratedToken = await _authTokenGenerator.GenerateJwtToken(existingUser);
                await _usersManager.SetAuthenticationTokenAsync(existingUser, "SQLServer", "AuthToken", geteratedToken);
                return (true, geteratedToken, null);
            }

            return (false, null, $"link_failed: {string.Join(", ", addLoginResult.Errors.Select(e => e.Description))}");
        }

        // Создаем нового пользователя
        _logger.LogInformation("Creating new user for {Provider} login with email {Email}", provider, email);

        var newUser = new ApplicationUser
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

        var createResult = await _usersManager.CreateAsync(newUser);

        if (!createResult.Succeeded)
        {
            return (false, null, $"user_creation_failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
        }

        // Получаем созданного пользователя
        var createdUser = await _usersManager.FindByEmailAsync(email);
        if (createdUser == null)
        {
            return (false, null, "user_not_found_after_creation");
        }

        // Добавляем внешний логин
        var addLoginResultForNew = await _usersManager.AddLoginAsync(createdUser,
            new UserLoginInfo(provider, providerKey, provider));

        if (!addLoginResultForNew.Succeeded)
        {
            // Откатываем создание пользователя
            await _usersManager.DeleteAsync(createdUser);
            return (false, null, $"external_login_add_failed: {string.Join(", ", addLoginResultForNew.Errors.Select(e => e.Description))}");
        }

        // Генерируем токен
        var token = await _authTokenGenerator.GenerateJwtToken(createdUser);
        await _usersManager.SetAuthenticationTokenAsync(createdUser, "SQLServer", "AuthToken", token);

        return (true, token, null);
    }

    // ✅ ВСПОМОГАТЕЛЬНЫЙ МЕТОД ДЛЯ ГЕНЕРАЦИИ УНИКАЛЬНОГО USERNAME
    private string GenerateUniqueUsername(string baseUsername)
    {
        // Очищаем username от пробелов и спецсимволов
        var cleanUsername = System.Text.RegularExpressions.Regex.Replace(baseUsername, @"[^a-zA-Z0-9_]", "");

        if (string.IsNullOrEmpty(cleanUsername))
        {
            cleanUsername = $"user_{DateTime.Now.Ticks}";
        }

        // Проверяем уникальность
        var existingUser = _usersManager.FindByNameAsync(cleanUsername).Result;
        if (existingUser != null)
        {
            return $"{cleanUsername}_{DateTime.Now.Ticks}";
        }

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
