using API.Auth;
using Domain.Auth;
using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using MimeKit;
using Settings;
using System.Net;

namespace API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _usersManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AuthTokenGenerator _authTokenGenerator;
    private readonly TwoFactorAuthEmailProcessor _twoFactorAuthEmailProcessor;

    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ILogger<AuthController> _logger;
    private readonly IOptionsMonitor<AuthSettings> _authSettingsOptionsMonitor;
    private readonly IOptionsMonitor<TokenValidationParameters> _tokenValidationParameters;
    private readonly IOptionsMonitor<EmailSettings> _emailSettings;

    public AuthController(IConfiguration configuration, UserManager<ApplicationUser> usersManager, IPasswordHasher<ApplicationUser> passwordHasher, ILogger<AuthController> logger, IOptionsMonitor<AuthSettings> authSettingsOptionsMonitor, IOptionsMonitor<EmailSettings> emailSettings, IOptionsMonitor<TokenValidationParameters> tokenValidationParameters, SignInManager<ApplicationUser> signInManager, AuthTokenGenerator authTokenGenerator, TwoFactorAuthEmailProcessor twoFactorAuthEmailProcessor)
    {
        _configuration = configuration;
        _usersManager = usersManager;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _authSettingsOptionsMonitor = authSettingsOptionsMonitor;
        _emailSettings = emailSettings;
        _tokenValidationParameters = tokenValidationParameters;
        _signInManager = signInManager;
        _authTokenGenerator = authTokenGenerator;
        _twoFactorAuthEmailProcessor = twoFactorAuthEmailProcessor;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("addExternalLogin")]
    public async Task<ActionResult> AddExternalLogin()
    {
        try
        {
            ApplicationUser? authUser = await _usersManager.FindByIdAsync(HttpContext.User.Claims.Single(b => b.Type == ClaimTypes.NameIdentifier).Value);
            if (authUser is null)
                return NotFound();

            IdentityResult identityResult = await _usersManager.AddLoginAsync(authUser, new UserLoginInfo("Google", authUser.Email, "Google"));

            if (identityResult.Succeeded)
                return Ok(identityResult);
            return BadRequest(identityResult);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("external-providers")]
    public async Task<ActionResult<IEnumerable<Microsoft.AspNetCore.Authentication.AuthenticationScheme>>> GetExternalProviders()
    {
        IEnumerable<Microsoft.AspNetCore.Authentication.AuthenticationScheme> externalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
        return Ok(externalProviders.Select(ep => new Domain.Auth.AuthenticationScheme(ep.Name, ep.DisplayName)));
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginModel loginModel)
    {
        if (loginModel is null)
            return BadRequest("Неверный логин");

        if (string.IsNullOrWhiteSpace(loginModel.UserEmail) || string.IsNullOrWhiteSpace(loginModel.Password))
            return BadRequest("Email и пароль должны быть указаны");

        ApplicationUser? userToCheckExistance = await _usersManager.FindByEmailAsync(loginModel.UserEmail);

        if (userToCheckExistance is null)
            return NotFound("Пользователь не зарегистрирован");

        PasswordVerificationResult passwordVerificationResult = _passwordHasher.VerifyHashedPassword(userToCheckExistance, userToCheckExistance.PasswordHash, loginModel.Password);

        if (passwordVerificationResult != PasswordVerificationResult.Success)
            return BadRequest("Неверный пароль");

        // If 2FA is enabled, send email and return specific response
        if (userToCheckExistance.TwoFactorEnabled is true)
        {
            string twoFactorAuthToken = await _usersManager.GenerateTwoFactorTokenAsync(userToCheckExistance, "Email");

            MimeMessage emailMessage = new();

            emailMessage.From.Add(new MailboxAddress(_emailSettings.CurrentValue.Sender.Name, _emailSettings.CurrentValue.Sender.Email));
            emailMessage.To.Add(new MailboxAddress("", userToCheckExistance.Email));
            emailMessage.Subject = "Confirm login";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"Your 2FA verification code is: <strong>{twoFactorAuthToken}</strong><br><br>" +
                       $"Enter this code to complete your login."
            };

            using (SmtpClient client = new())
            {
                await client.ConnectAsync(_emailSettings.CurrentValue.Host, _emailSettings.CurrentValue.Port, _emailSettings.CurrentValue.UseSsl);
                await client.AuthenticateAsync(_emailSettings.CurrentValue.UserName, _emailSettings.CurrentValue.Password);
                _ = await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

            // Return specific response indicating 2FA is required
            return Ok(new LoginResponseModel(userToCheckExistance.Id.ToString(), string.Empty, 0, string.Empty, userToCheckExistance.TwoFactorEnabled));
        }

        string accessToken = await _authTokenGenerator.GenerateAccessToken(userToCheckExistance);
        string refreshToken = await _authTokenGenerator.GenerateRefreshToken(userToCheckExistance);

        IdentityResult settingAccessTokenResult = await _usersManager.SetAuthenticationTokenAsync(userToCheckExistance, "SQLServer", "AccessToken", accessToken);
        IdentityResult settingRefreshTokenResult = await _usersManager.SetAuthenticationTokenAsync(userToCheckExistance, "SQLServer", "RefreshToken", refreshToken);

        if (settingAccessTokenResult.Succeeded && settingRefreshTokenResult.Succeeded)
            return Ok(new TokenResponse(true, accessToken, _authSettingsOptionsMonitor.CurrentValue.AccessTokenLifetimeMinutes, refreshToken));

        return StatusCode(500, "Authentication token setting has been failed");
    }

    [HttpPost("ConfirmLoginViaEmail")]
    public async Task<ActionResult> ConfirmLoginViaEmail(ConfirmLoginModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserId) || string.IsNullOrWhiteSpace(model.TwoFactorToken))
        {
            _logger.LogError("User ID and token are required");

            return BadRequest("User ID and token are required");
        }
        _logger.LogInformation("ConfirmLoginViaEmail called with UserId: {UserId} and TwoFactorToken: {TwoFactorToken}", model.UserId, model.TwoFactorToken);

        ApplicationUser? userToCheckExistance = await _usersManager.FindByIdAsync(model.UserId);

        if (userToCheckExistance is null)
        {
            _logger.LogError("User with ID {UserId} not found", model.UserId);
            return NotFound();
        }

        _logger.LogInformation("User to check existance - {UserId}, {Email}", userToCheckExistance?.Id, userToCheckExistance?.Email);

        bool isValidTwoFactorToken = await _usersManager.VerifyTwoFactorTokenAsync(userToCheckExistance, "Email", model.TwoFactorToken);

        _logger.LogInformation("Is valid 2FA token for user {UserId}: {IsValidTwoFactorToken}", userToCheckExistance.Id, isValidTwoFactorToken);

        if (isValidTwoFactorToken)
        {
            string accessToken = await _authTokenGenerator.GenerateAccessToken(userToCheckExistance);
            string refreshToken = await _authTokenGenerator.GenerateRefreshToken(userToCheckExistance);

            _logger.LogInformation("Generated Access token for user {UserId}: {TokenString}", userToCheckExistance.Id, accessToken);
            _logger.LogInformation("Generated Refresh token for user {UserId}: {TokenString}", userToCheckExistance.Id, refreshToken);

            IdentityResult settingsAccessTokenResult = await _usersManager.SetAuthenticationTokenAsync(userToCheckExistance, "SQLServer", "AccessToken", accessToken);
            IdentityResult settingsRefreshTokenResult = await _usersManager.SetAuthenticationTokenAsync(userToCheckExistance, "SQLServer", "RefreshToken", refreshToken);

            if (settingsAccessTokenResult.Succeeded && settingsRefreshTokenResult.Succeeded)
            {
                _logger.LogInformation("Authentication token set successfully for user {UserId} - {tokenString}", userToCheckExistance.Id, accessToken);
                _logger.LogInformation("Authentication token set successfully for user {UserId} - {tokenString}", userToCheckExistance.Id, refreshToken);
                return Ok(new TokenResponse(true, accessToken, _authSettingsOptionsMonitor.CurrentValue.AccessTokenLifetimeMinutes, refreshToken));
            }

            if (settingsAccessTokenResult.Errors.Any())
                _logger.LogError("Failed to set authentication token for user {UserId}. Errors: {Errors}", userToCheckExistance.Id, string.Join(", ", settingsAccessTokenResult.Errors.Select(e => $"{e.Code}: {e.Description}")));
            if (settingsRefreshTokenResult.Errors.Any())
                _logger.LogError("Failed to set authentication token for user {UserId}. Errors: {Errors}", userToCheckExistance.Id, string.Join(", ", settingsRefreshTokenResult.Errors.Select(e => $"{e.Code}: {e.Description}")));

            return BadRequest("Failed to set authentication token");
        }

        return BadRequest("Invalid 2FA token");
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Logout()
    {
        try
        {
            ApplicationUser authorizedApplicationUser = await _usersManager.FindByIdAsync(User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value);
            IdentityResult accessTokenLogoutResult = await _usersManager.RemoveAuthenticationTokenAsync(authorizedApplicationUser, "SQLServer", "AccessToken");
            IdentityResult refreshTokenlogoutResult = await _usersManager.RemoveAuthenticationTokenAsync(authorizedApplicationUser, "SQLServer", "RefreshToken");
            if (accessTokenLogoutResult.Succeeded && refreshTokenlogoutResult.Succeeded)
                return Ok();

            _logger.LogError("Ошибка отзыва токена авторизации");
            return StatusCode(500, "Ошибка отзыва токена авторизации");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return StatusCode(500, $"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(RegisterModel registerModel)
    {
        ApplicationUser? userToCheckExistance = await _usersManager.FindByEmailAsync(registerModel.UserEmail);

        if (userToCheckExistance is not null)
            return BadRequest($"Пользователь с {registerModel.UserEmail} уже существует");
        IQueryable<ApplicationUser> registeredUsers = _usersManager.Users;
        userToCheckExistance = registeredUsers.FirstOrDefault(b => b.NormalizedUserName == registerModel.UserName.ToUpperInvariant());
        if (userToCheckExistance is not null)
            return BadRequest($"Пользователь с логином {registerModel.UserName} уже существует");

        if (!string.Equals(registerModel.Password, registerModel.PasswordConfirmation))
            return BadRequest("Пароль не совпадает с подтверждением пароля");

        string passwordHash = _passwordHasher.HashPassword(null, registerModel.Password);

        ApplicationUser? user = new()
        {
            Email = registerModel.UserEmail,
            PasswordHash = passwordHash,
            EmailConfirmed = false,
            NormalizedEmail = registerModel.UserEmail.ToUpperInvariant(),
            NormalizedUserName = registerModel.UserName.ToUpperInvariant(),
            PhoneNumber = registerModel.PhoneNumber,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            UserName = registerModel.UserName,
            SecurityStamp = DateTime.Now.ToString()
        };

        IdentityResult userCreationResult = await _usersManager.CreateAsync(user);

        if (userCreationResult.Succeeded)
        {
            if (string.Equals(registerModel.UserEmail, _authSettingsOptionsMonitor.CurrentValue.AdminEmail) && string.Equals(registerModel.Password, _authSettingsOptionsMonitor.CurrentValue.AdminPassword))
            {
                ApplicationUser? userToBindToAdminRole = await _usersManager.FindByEmailAsync(user.Email);
                IdentityResult addingToAdminRoleIdentityResult = await _usersManager.AddToRoleAsync(userToBindToAdminRole, "Admin");
                if (!addingToAdminRoleIdentityResult.Succeeded)
                {
                    _logger.LogError($"Ошибка добавления к роли администратора. {string.Join(", ", addingToAdminRoleIdentityResult.Errors.Select(b => $"{b.Code}, {b.Description}"))}");
                }
            }

            user = await _usersManager.FindByEmailAsync(registerModel.UserEmail);

            string code = WebUtility.UrlEncode(await _usersManager.GenerateEmailConfirmationTokenAsync(user));
            string? callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

            MimeMessage emailMessage = new();

            emailMessage.From.Add(new MailboxAddress(_emailSettings.CurrentValue.Sender.Name, _emailSettings.CurrentValue.Sender.Email));
            emailMessage.To.Add(new MailboxAddress("", user.Email));
            emailMessage.Subject = "Confirm email";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"Confirm email: go to email confirmation <a href=\"{callbackUrl}\">link</a> to confirm your email"
            };

            using (SmtpClient client = new())
            {
                await client.ConnectAsync(_emailSettings.CurrentValue.Host, _emailSettings.CurrentValue.Port, _emailSettings.CurrentValue.UseSsl);
                await client.AuthenticateAsync(_emailSettings.CurrentValue.UserName, _emailSettings.CurrentValue.Password);
                _ = await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }

            return Ok("Email verification has been set");
        }

        return StatusCode(500, "User creation error");
    }



    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            return BadRequest("UserId and code are required");

        // URL decode the code if needed
        code = WebUtility.UrlDecode(code);

        ApplicationUser? user = await _usersManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound("User not found");

        IdentityResult emailConfirmationResult = await _usersManager.ConfirmEmailAsync(user, code);

        if (!emailConfirmationResult.Succeeded)
            return StatusCode(StatusCodes.Status400BadRequest, userId);

        return Ok($"Email {user.Email} подтверждён.");
    }

    [HttpPost("assignToAdmin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult> AssignToAdmin(string humanToAssignToAdminEmail)
    {
        ApplicationUser? humanToAssignToAdmin = await _usersManager.FindByEmailAsync(humanToAssignToAdminEmail);

        if (humanToAssignToAdmin is null)
            return NotFound("Human to assign to admin not found");

        else
        {
            IdentityResult identityResult = await _usersManager.AddToRoleAsync(humanToAssignToAdmin, "Admin");
            if (identityResult is null)
                return NotFound();
            if (!identityResult.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, identityResult);
            return Ok(identityResult);
        }
    }

    [HttpPost("resetPassword")]
    public async Task<ActionResult> ResetPassword(Domain.Auth.ResetPasswordModel resetPasswordModel)
    {
        ApplicationUser user = await _usersManager.FindByEmailAsync(resetPasswordModel.Email);
        if (user is null)
            return NotFound();

        string resetPasswordToken = await _usersManager.GeneratePasswordResetTokenAsync(user);

        MimeMessage emailMessage = new();

        emailMessage.From.Add(new MailboxAddress(_emailSettings.CurrentValue.Sender.Name, _emailSettings.CurrentValue.Sender.Email));
        emailMessage.To.Add(new MailboxAddress("", user.Email));
        emailMessage.Subject = "Reset password";
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $"Reset password token - {resetPasswordToken}"
        };

        using (SmtpClient client = new())
        {
            await client.ConnectAsync(_emailSettings.CurrentValue.Host, _emailSettings.CurrentValue.Port, _emailSettings.CurrentValue.UseSsl);
            await client.AuthenticateAsync(_emailSettings.CurrentValue.UserName, _emailSettings.CurrentValue.Password);
            _ = await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }

        return Ok($"Email with reset password token has been send to {resetPasswordModel.Email}");
    }

    [HttpPost("resetPasswordConfirm")]
    public async Task<ActionResult> ResetPasswordConfirm(Domain.Auth.ResetPasswordConfirmModel resetPasswordModel)
    {
        ApplicationUser user = await _usersManager.FindByEmailAsync(resetPasswordModel.Email);
        if (user is null)
            return NotFound();
        IdentityResult passwordResettingResult = await _usersManager.ResetPasswordAsync(user, resetPasswordModel.ResetPasswordToken, resetPasswordModel.NewPassword);
        if (passwordResettingResult.Succeeded)
            return Ok("Password has been changed successfully");
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("setTwoFactorEnabled")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> SetTwoFactorEnabled(Domain.Auth.SetTwoFactorEnabledModel setTwoFactorEnabledModel)
    {
        Claim? userId = User.FindFirst(ClaimTypes.NameIdentifier);
        ApplicationUser applicationUser = await _usersManager.FindByIdAsync(userId.Value);
        if (applicationUser is null)
            return NotFound();
        IdentityResult settingTwoFactorEnabledResult = await _usersManager.SetTwoFactorEnabledAsync(applicationUser, setTwoFactorEnabledModel.TwoFactorEnabled);
        if (settingTwoFactorEnabledResult.Succeeded)
            return Ok("Two factor enabled fact has been changed successfully");
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("addPassword/{password}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> AddPasswordAsync(string password)
    {
        _logger.LogInformation("addPassword");

        try
        {
            string emailClaimValue = User.Claims.SingleOrDefault(b => b.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(emailClaimValue))
            {
                _logger.LogWarning("Email claim not found in user claims");
                return BadRequest("Email claim not found");
            }

            ApplicationUser? userToCheckExistance = await _usersManager.FindByEmailAsync(emailClaimValue);

            if (userToCheckExistance is null)
            {
                _logger.LogWarning("userToCheckExistance is null");
                return NotFound();
            }
            if (string.IsNullOrWhiteSpace(userToCheckExistance.PasswordHash))
            {
                _ = await _usersManager.AddPasswordAsync(userToCheckExistance, password);

                return Ok();
            }

            return BadRequest("Password is already set for this user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при добавлении пароля: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return StatusCode(500, $"Ошибка при добавлении пароля: {ex.Message}");
        }

    }

    [HttpGet("login-google")]
    public async Task<ActionResult> LoginViaGoogle()
    {
        try
        {
            string redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            _logger.LogInformation("AllowRefresh: {AllowRefresh}", properties.AllowRefresh);
            _logger.LogInformation("ExpiresUtc: {ExpiresUtc}", properties.ExpiresUtc);
            _logger.LogInformation("IsPersistent: {IsPersistent}", properties.IsPersistent);
            _logger.LogInformation("IssuedUtc: {IssuedUtc}", properties.IssuedUtc);
            _logger.LogInformation("RedirectUri: {RedirectUri}", properties.RedirectUri);
            _logger.LogInformation("Items: {Items}", string.Join(", ", properties.Items.Select(kvp => $"{kvp.Key}: {kvp.Value}")));
            return Challenge(properties, "Google");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при попытке входа через Google: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return StatusCode(500, $"Ошибка при попытке входа через Google: {ex.Message}");
        }
    }

    [HttpGet("google-callback")]
    public async Task<ActionResult> GoogleCallback()
    {
        try
        {
            _logger.LogInformation("google-callback");

            AuthenticateResult result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal is null)
            {
                _logger.LogWarning("Google authentication failed");
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=google_auth_failed");
            }

            // Извлекаем данные
            string? email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            string? googleUserId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? phoneNumber = result.Principal.FindFirst(ClaimTypes.MobilePhone)?.Value;
            string? name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleUserId))
            {
                _logger.LogError("Email or GoogleUserId is null");
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=missing_required_data");
            }

            _logger.LogInformation("Processing Google login for email: {Email}, GoogleUserId: {GoogleUserId}", email, googleUserId);

            // ✅ ВЫЗЫВАЕМ УНИВЕРСАЛЬНЫЙ МЕТОД
            TokenResponse tokenResponse = await _twoFactorAuthEmailProcessor.ProcessExternalLoginAsync(
                provider: "Google",
                providerKey: googleUserId,
                email: email,
                name: name,
                phoneNumber: phoneNumber
            );

            if (tokenResponse.Success)
            {
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/google-callback?Token={tokenResponse.AccessToken}");
            }

            return Redirect($"{Request.Scheme}://{Request.Host}/login?error=ОШИБКА");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Google callback: {ex.Message}");
            return Redirect($"{Request.Scheme}://{Request.Host}/login?error={WebUtility.UrlEncode(ex.Message)}");
        }
    }

    [HttpGet("login-github")]
    public async Task<ActionResult> LoginViaGithub()
    {
        try
        {
            string redirectUrl = Url.Action(nameof(GithubCallback), "Auth", null, Request.Scheme);
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("GitHub", redirectUrl);
            _logger.LogInformation("AllowRefresh: {AllowRefresh}", properties.AllowRefresh);
            _logger.LogInformation("ExpiresUtc: {ExpiresUtc}", properties.ExpiresUtc);
            _logger.LogInformation("IsPersistent: {IsPersistent}", properties.IsPersistent);
            _logger.LogInformation("IssuedUtc: {IssuedUtc}", properties.IssuedUtc);
            _logger.LogInformation("RedirectUri: {RedirectUri}", properties.RedirectUri);
            _logger.LogInformation("Items: {Items}", string.Join(", ", properties.Items.Select(kvp => $"{kvp.Key}: {kvp.Value}")));
            return Challenge(properties, "GitHub");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при попытке входа через GitHub: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return StatusCode(500, $"Ошибка при попытке входа через GitHub: {ex.Message}");
        }
    }

    [HttpGet("github-callback")]
    public async Task<ActionResult> GithubCallback()
    {
        try
        {
            _logger.LogInformation("github-callback");

            // Используем "cookie" (с маленькой буквы) - ту, что только что создали
            AuthenticateResult result = await HttpContext.AuthenticateAsync("cookie");

            if (!result.Succeeded || result.Principal is null)
            {
                _logger.LogWarning("GitHub authentication failed");
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=github_auth_failed");
            }

            // Извлекаем данные
            string? email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            string? githubUserId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? phoneNumber = result.Principal.FindFirst(ClaimTypes.MobilePhone)?.Value;
            string? name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(githubUserId))
            {
                _logger.LogError("Email or GithubUserId is null");
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=missing_required_data");
            }

            _logger.LogInformation("Processing Github login for email: {Email}, GithubUserId: {GithubUserId}", email, githubUserId);

            // ✅ ВЫЗЫВАЕМ УНИВЕРСАЛЬНЫЙ МЕТОД
            TokenResponse tokenResponse = await _twoFactorAuthEmailProcessor.ProcessExternalLoginAsync(
                provider: "GitHub",
                providerKey: githubUserId,
                email: email,
                name: name,
                phoneNumber: phoneNumber
            );

            if (tokenResponse.Success)
            {
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/github-callback?Token={tokenResponse.AccessToken}");
            }

            return Redirect($"{Request.Scheme}://{Request.Host}/login?error=Ошибка");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GitHub callback: {ex.Message}");
            return Redirect($"{Request.Scheme}://{Request.Host}/login?error={WebUtility.UrlEncode(ex.Message)}");
        }
    }

    [HttpGet("login-vkontakte")]
    public async Task<ActionResult> LoginVkontakte()
    {
        try
        {
            string redirectUrl = Url.Action(nameof(VkontakteCallback), "Auth", null, Request.Scheme);
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("Vkontakte", redirectUrl);
            _logger.LogInformation("AllowRefresh: {AllowRefresh}", properties.AllowRefresh);
            _logger.LogInformation("ExpiresUtc: {ExpiresUtc}", properties.ExpiresUtc);
            _logger.LogInformation("IsPersistent: {IsPersistent}", properties.IsPersistent);
            _logger.LogInformation("IssuedUtc: {IssuedUtc}", properties.IssuedUtc);
            _logger.LogInformation("RedirectUri: {RedirectUri}", properties.RedirectUri);
            _logger.LogInformation("Items: {Items}", string.Join(", ", properties.Items.Select(kvp => $"{kvp.Key}: {kvp.Value}")));
            return Challenge(properties, "Vkontakte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при попытке входа через Vkontakte: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return StatusCode(500, $"Ошибка при попытке входа через Vkontakte: {ex.Message}");
        }
    }

    [HttpGet("vkontakte-callback")]
    public async Task<ActionResult> VkontakteCallback()
    {
        try
        {
            _logger.LogInformation("vkontakte-callback");

            // Используем "cookie" (с маленькой буквы) - ту, что только что создали
            AuthenticateResult result = await HttpContext.AuthenticateAsync("cookie");

            if (!result.Succeeded || result.Principal is null)
            {
                _logger.LogWarning("Vkontakte authentication failed");
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=vkontakte_auth_failed");
            }

            // Извлекаем данные
            string? email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            string? vkontakteUserId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? phoneNumber = result.Principal.FindFirst(ClaimTypes.MobilePhone)?.Value;
            string? name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(vkontakteUserId))
            {
                _logger.LogError("Email or VkontakteUserId is null");
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=missing_required_data");
            }

            _logger.LogInformation("Processing Vkontakte login for email: {Email}, VkontakteUserId: {VkontakteUserId}", email, vkontakteUserId);

            // ✅ ВЫЗЫВАЕМ УНИВЕРСАЛЬНЫЙ МЕТОД
            TokenResponse tokenResponse = await _twoFactorAuthEmailProcessor.ProcessExternalLoginAsync(
                provider: "Vkontakte",
                providerKey: vkontakteUserId,
                email: email,
                name: name,
                phoneNumber: phoneNumber
            );

            if (tokenResponse.Success)
            {
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/vkontakte-callback?Token={tokenResponse.AccessToken}");
            }

            return Redirect($"{Request.Scheme}://{Request.Host}/login?error=ОШИБКА");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Vkontakte callback: {ex.Message}");
            return Redirect($"{Request.Scheme}://{Request.Host}/login?error={WebUtility.UrlEncode(ex.Message)}");
        }
    }


    [HttpGet("current-user")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<ApplicationUser>> GetCurrentUserAsync()
    {
        string? emailClaimValue = User.Claims.SingleOrDefault(b => b.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(emailClaimValue))
        {
            _logger.LogWarning("Email claim not found in user claims");
            return null;
        }
        ApplicationUser? user = await _usersManager.FindByEmailAsync(emailClaimValue);
        if (user is null)
        {
            _logger.LogWarning("User with email {Email} not found", emailClaimValue);
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost("set-password")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
    {
        string? emailClaimValue = User.Claims.SingleOrDefault(b => b.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(emailClaimValue))
        {
            _logger.LogWarning("Email claim not found in user claims");
            return BadRequest("Email claim not found");
        }
        ApplicationUser? user = await _usersManager.FindByEmailAsync(emailClaimValue);
        if (user is null)
        {
            _logger.LogWarning("User with email {Email} not found", emailClaimValue);
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            bool isValidPassword = await _usersManager.CheckPasswordAsync(user, changePasswordModel.CurrentPassword);

            if (!isValidPassword)
                return BadRequest("Current password is incorrect");

            IdentityResult changePasswordResult = await _usersManager.ChangePasswordAsync(user, changePasswordModel.CurrentPassword, changePasswordModel.NewPassword);
            if (changePasswordResult.Succeeded)
                return Ok("Password has been changed successfully");

            return StatusCode(StatusCodes.Status500InternalServerError, changePasswordResult);
        }
        else
        {
            IdentityResult addPasswordResult = await _usersManager.AddPasswordAsync(user, changePasswordModel.NewPassword);
            if (addPasswordResult.Succeeded)
                return Ok("Password has been added successfully");

            return StatusCode(StatusCodes.Status500InternalServerError, addPasswordResult);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return BadRequest("Refresh token is required");

        try
        {
            // Получаем principal из истекшего токена (мы все еще можем его прочитать)
            ClaimsPrincipal? principal = null;
            string? accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(accessToken))
            {
                JwtSecurityTokenHandler tokenHandler = new();
                try
                {
                    // Игнорируем срок действия токена при чтении
                    principal = tokenHandler.ValidateToken(accessToken,
                        new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(_authSettingsOptionsMonitor.CurrentValue.RefreshSecret)),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = false // Не проверяем срок действия
                        }, out _);
                }
                catch
                {
                    // Если не можем прочитать токен, пробуем найти пользователя по refresh token
                }
            }

            ApplicationUser? user = null;

            if (principal != null)
            {
                string? userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    user = await _usersManager.FindByIdAsync(userId);
                }
            }

            // Если пользователь не найден по токену, ищем по refresh token в базе
            if (user == null)
            {
                // Поиск пользователя, у которого есть такой refresh token
                user = _usersManager.Users
                    .AsEnumerable() // Загружаем в память для поиска по токенам
                    .FirstOrDefault(u =>
                    {
                        string? storedToken = _usersManager.GetAuthenticationTokenAsync(u, "SQLServer", "RefreshToken").Result;
                        return storedToken == request.RefreshToken;
                    });
            }

            if (user == null)
                return BadRequest("Invalid refresh token");

            // Проверяем, что refresh token совпадает с сохраненным
            string? storedRefreshToken = await _usersManager.GetAuthenticationTokenAsync(user, "SQLServer", "RefreshToken");

            if (storedRefreshToken != request.RefreshToken)
                return BadRequest("Refresh token mismatch");

            // Генерируем новые токены
            string newAccessToken = await _authTokenGenerator.GenerateAccessToken(user);
            string newRefreshToken = await _authTokenGenerator.GenerateRefreshToken(user);

            // Сохраняем новые токены
            _ = await _usersManager.SetAuthenticationTokenAsync(user, "SQLServer", "AccessToken", newAccessToken);
            _ = await _usersManager.SetAuthenticationTokenAsync(user, "SQLServer", "RefreshToken", newRefreshToken);

            return Ok(new TokenResponse(true, newAccessToken,
                _authSettingsOptionsMonitor.CurrentValue.AccessTokenLifetimeMinutes, newRefreshToken));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, "Internal server error during token refresh");
        }
    }
}
