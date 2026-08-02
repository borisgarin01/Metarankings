using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using IdentityLibrary.Repositories.Tokens.RefreshTokens.Interfaces;
using IdentityLibrary.Services.Classes;
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

    public async Task<AuthResponseDto> ProcessExternalLoginAsync(
        string provider,
        string providerKey,
        string email,
        string? name = null,
        string? phoneNumber = null)
    {
        try
        {
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                loginProvider: provider,
                providerKey: providerKey,
                isPersistent: true);

            _logger.LogInformation("ExternalLoginSignInAsync result for {Provider}: {Result}", provider, signInResult);

            if (signInResult.Succeeded)
            {
                var user = await _usersManager.FindByLoginAsync(provider, providerKey);
                if (user != null)
                {
                    await _refreshTokensRepo.RevokeAllByUserIdAsync(Convert.ToInt64(user.Id));

                    var newAccessToken = await _authTokenGenerator.GenerateAccessToken(user);
                    var newRefreshTokenValue = _authTokenGenerator.GenerateRefreshToken();

                    var newRefreshToken = new RefreshToken(0, Convert.ToInt64(user.Id), newRefreshTokenValue, false, DateTime.UtcNow);
                    await _refreshTokensRepo.CreateAsync(newRefreshToken);

                    return new AuthResponseDto(true, false, string.Empty, newAccessToken, newRefreshTokenValue);
                }
                return new AuthResponseDto(false, false, "user_not_found", string.Empty, string.Empty);
            }

            if (signInResult.RequiresTwoFactor)
            {
                var user = await _usersManager.FindByLoginAsync(provider, providerKey);
                if (user != null)
                {
                    string twoFactorToken = await _usersManager.GenerateTwoFactorTokenAsync(user, "Email");
                    await SendTwoFactorEmailAsync(user.Email, twoFactorToken);

                    return new AuthResponseDto(false, true, "requires_two_factor", string.Empty, string.Empty);
                }
                return new AuthResponseDto(false, false, "user_not_found_for_2fa", string.Empty, string.Empty);
            }

            if (signInResult.IsLockedOut)
                return new AuthResponseDto(false, false, "account_locked", string.Empty, string.Empty);

            return await CreateOrLinkUserAsync(provider, providerKey, email, name, phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing external login for {Provider}", provider);
            return new AuthResponseDto(false, false, $"processing_error: {ex.Message}", string.Empty, string.Empty);
        }
    }

    private async Task<AuthResponseDto> CreateOrLinkUserAsync(
        string provider,
        string providerKey,
        string email,
        string? name = null,
        string? phoneNumber = null)
    {
        var existingUser = await _usersManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            _logger.LogInformation("Linking {Provider} account to existing user {Email}", provider, email);

            var existingLogin = await _usersManager.FindByLoginAsync(provider, providerKey);
            if (existingLogin != null)
                return new AuthResponseDto(false, false, "external_account_already_linked", string.Empty, string.Empty);

            var addLoginResult = await _usersManager.AddLoginAsync(existingUser,
                new UserLoginInfo(provider, providerKey, provider));

            if (addLoginResult.Succeeded)
            {
                await _refreshTokensRepo.RevokeAllByUserIdAsync(Convert.ToInt64(existingUser.Id));

                var linkedAccessToken = await _authTokenGenerator.GenerateAccessToken(existingUser);
                var linkedRefreshTokenValue = _authTokenGenerator.GenerateRefreshToken();

                var linkedRefreshToken = new RefreshToken(0, Convert.ToInt64(existingUser.Id), linkedRefreshTokenValue, false, DateTime.UtcNow);
                await _refreshTokensRepo.CreateAsync(linkedRefreshToken);

                return new AuthResponseDto(true, false, string.Empty, linkedAccessToken, linkedRefreshTokenValue);
            }

            return new AuthResponseDto(false, false, $"link_failed: {string.Join(", ", addLoginResult.Errors.Select(e => e.Description))}", string.Empty, string.Empty);
        }

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
            return new AuthResponseDto(false, false, $"user_creation_failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}", string.Empty, string.Empty);

        var createdUser = await _usersManager.FindByEmailAsync(email);
        if (createdUser == null)
            return new AuthResponseDto(false, false, "user_not_found_after_creation", string.Empty, string.Empty);

        var addLoginResultForNew = await _usersManager.AddLoginAsync(createdUser,
            new UserLoginInfo(provider, providerKey, provider));

        if (!addLoginResultForNew.Succeeded)
        {
            await _usersManager.DeleteAsync(createdUser);
            return new AuthResponseDto(false, false, $"external_login_add_failed: {string.Join(", ", addLoginResultForNew.Errors.Select(e => e.Description))}", string.Empty, string.Empty);
        }

        await _refreshTokensRepo.RevokeAllByUserIdAsync(Convert.ToInt64(createdUser.Id));

        var createdAccessToken = await _authTokenGenerator.GenerateAccessToken(createdUser);
        var createdRefreshTokenValue = _authTokenGenerator.GenerateRefreshToken();

        var createdRefreshToken = new RefreshToken(0, Convert.ToInt64(createdUser.Id), createdRefreshTokenValue, false, DateTime.UtcNow);
        await _refreshTokensRepo.CreateAsync(createdRefreshToken);

        return new AuthResponseDto(true, false, string.Empty, createdAccessToken, createdRefreshTokenValue);
    }

    private string GenerateUniqueUsername(string baseUsername)
    {
        string cleanUsername = System.Text.RegularExpressions.Regex.Replace(baseUsername, @"[^a-zA-Z0-9_]", "");

        if (string.IsNullOrEmpty(cleanUsername))
            cleanUsername = $"user_{DateTime.Now.Ticks}";

        var existingUser = _usersManager.FindByNameAsync(cleanUsername).Result;
        if (existingUser != null)
            return $"{cleanUsername}_{DateTime.Now.Ticks}";

        return cleanUsername.ToLowerInvariant();
    }

    private async Task SendTwoFactorEmailAsync(string email, string token)
    {
        try
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.CurrentValue.Sender.Name, _emailSettings.CurrentValue.Sender.Email));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Confirm login";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"Your 2FA verification code is: <strong>{token}</strong><br><br>" +
                       $"Enter this code to complete your login."
            };

            using var client = new SmtpClient();
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