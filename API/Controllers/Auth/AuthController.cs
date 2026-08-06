using API.Auth;
using Domain.Auth;
using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using IdentityLibrary.Repositories.Tokens.RefreshTokens.Interfaces;
using IdentityLibrary.Services.Classes;
using IdentityLibrary.Services.Interfaces;
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
    private readonly ITokensService _tokenService;
    private readonly IRefreshTokensRepository _refreshTokensRepo;

    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ILogger<AuthController> _logger;
    private readonly IOptionsMonitor<AuthSettings> _authSettingsOptionsMonitor;
    private readonly IOptionsMonitor<TokenValidationParameters> _tokenValidationParameters;
    private readonly IOptionsMonitor<EmailSettings> _emailSettings;

    public AuthController(IConfiguration configuration, UserManager<ApplicationUser> usersManager, IPasswordHasher<ApplicationUser> passwordHasher, ILogger<AuthController> logger, IOptionsMonitor<AuthSettings> authSettingsOptionsMonitor, IOptionsMonitor<EmailSettings> emailSettings, IOptionsMonitor<TokenValidationParameters> tokenValidationParameters, SignInManager<ApplicationUser> signInManager, AuthTokenGenerator authTokenGenerator, TwoFactorAuthEmailProcessor twoFactorAuthEmailProcessor, ITokensService tokenService, IRefreshTokensRepository refreshTokensRepo)
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
        _tokenService = tokenService;
        _refreshTokensRepo = refreshTokensRepo;
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

        ApplicationUser? user = await _usersManager.FindByEmailAsync(loginModel.UserEmail);

        if (user is null)
            return NotFound("Пользователь не зарегистрирован");

        bool isValidPassword = await _usersManager.CheckPasswordAsync(user, loginModel.Password);

        if (!isValidPassword)
            return BadRequest("Неверный пароль");

        if (user.TwoFactorEnabled)
        {
            string token = await _usersManager.GenerateTwoFactorTokenAsync(user, "Email");
            _logger.LogInformation("2FA code for {Email}: {Code}", user.Email, token);

            return Ok(new AuthResponseDto(false, true, "2FA required", string.Empty, string.Empty));
        }

        await _refreshTokensRepo.RevokeAllByUserIdAsync(Convert.ToInt64(user.Id));

        string accessToken = await _authTokenGenerator.GenerateAccessToken(user);
        string refreshTokenValue = _authTokenGenerator.GenerateRefreshToken();

        IdentityLibrary.DTOs.RefreshToken refreshToken = new IdentityLibrary.DTOs.RefreshToken(0, Convert.ToInt64(user.Id), refreshTokenValue, false, DateTime.UtcNow);
        await _refreshTokensRepo.CreateAsync(refreshToken);

        return Ok(new AuthResponseDto(true, false, string.Empty, accessToken, refreshTokenValue));
    }

    [HttpPost("ConfirmLoginViaEmail")]
    public async Task<ActionResult> ConfirmLoginViaEmail(ConfirmLoginModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserId) || string.IsNullOrWhiteSpace(model.TwoFactorToken))
        {
            _logger.LogError("User ID and token are required");
            return BadRequest("User ID and token are required");
        }

        ApplicationUser? user = await _usersManager.FindByIdAsync(model.UserId);

        if (user is null)
        {
            _logger.LogError("User with ID {UserId} not found", model.UserId);
            return NotFound();
        }

        bool isValidTwoFactorToken = await _usersManager.VerifyTwoFactorTokenAsync(user, "Email", model.TwoFactorToken);

        if (!isValidTwoFactorToken)
        {
            _logger.LogWarning("Invalid 2FA token for user {UserId}", user.Id);
            return BadRequest("Invalid 2FA token");
        }

        await _refreshTokensRepo.RevokeAllByUserIdAsync(Convert.ToInt64(user.Id));

        string accessToken = await _authTokenGenerator.GenerateAccessToken(user);
        string refreshTokenValue = _authTokenGenerator.GenerateRefreshToken();

        IdentityLibrary.DTOs.RefreshToken refreshToken = new IdentityLibrary.DTOs.RefreshToken(0, Convert.ToInt64(user.Id), refreshTokenValue, false, DateTime.UtcNow);
        await _refreshTokensRepo.CreateAsync(refreshToken);

        return Ok(new AuthResponseDto(true, false, string.Empty, accessToken, refreshTokenValue));
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Logout()
    {
        try
        {
            Claim? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
                return Unauthorized();

            ApplicationUser? user = await _usersManager.FindByIdAsync(userIdClaim.Value);
            if (user is null)
                return NotFound();

            await _refreshTokensRepo.RevokeAllByUserIdAsync(Convert.ToInt64(user.Id));

            return Ok("Logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return BadRequest("Refresh token is required");

        try
        {
            IdentityLibrary.DTOs.RefreshToken? storedToken = await _refreshTokensRepo.GetByValueAsync(request.RefreshToken);

            if (storedToken is null)
            {
                _logger.LogWarning("Invalid refresh token: {Token}", request.RefreshToken);
                return BadRequest("Invalid refresh token");
            }

            ApplicationUser? user = await _usersManager.FindByIdAsync(storedToken.UserId.ToString());
            if (user is null)
            {
                _logger.LogWarning("User not found for refresh token: {Token}", request.RefreshToken);
                return BadRequest("User not found");
            }

            await _refreshTokensRepo.RevokeAsync(storedToken.Id);

            string newAccessToken = await _authTokenGenerator.GenerateAccessToken(user);
            string newRefreshToken = _authTokenGenerator.GenerateRefreshToken();

            IdentityLibrary.DTOs.RefreshToken newToken = new IdentityLibrary.DTOs.RefreshToken(0, Convert.ToInt64(user.Id), newRefreshToken, false, DateTime.UtcNow);
            await _refreshTokensRepo.CreateAsync(newToken);

            return Ok(new AuthResponseDto(true, false, string.Empty, newAccessToken, newRefreshToken));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, "Internal server error during token refresh");
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
            AuthResponseDto tokenResponse = await _twoFactorAuthEmailProcessor.ProcessExternalLoginAsync(
                provider: "Google",
                providerKey: googleUserId,
                email: email,
                name: name,
                phoneNumber: phoneNumber
            );

            if (tokenResponse.IsAuthSuccessful)
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
            AuthResponseDto tokenResponse = await _twoFactorAuthEmailProcessor.ProcessExternalLoginAsync(
                provider: "GitHub",
                providerKey: githubUserId,
                email: email,
                name: name,
                phoneNumber: phoneNumber
            );

            if (tokenResponse.IsAuthSuccessful)
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

    [HttpGet("login-vkid")]
    public async Task<ActionResult> LoginVKID()
    {
        try
        {
            string redirectUrl = Url.Action(nameof(VKIDCallback), "Auth", null, Request.Scheme);
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("VK IDVK ID", redirectUrl);
            _logger.LogInformation("VK ID login initiated");
            return Challenge(properties, "VK ID");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при попытке входа через VK ID: {ex.Message}");
            return StatusCode(500, $"Ошибка при попытке входа через VK ID: {ex.Message}");
        }
    }

    [HttpGet("vkid-callback")]
    public async Task<ActionResult> VKIDCallback()
    {
        try
        {
            _logger.LogInformation("vkid-callback");

            AuthenticateResult result = await HttpContext.AuthenticateAsync("cookie");

            if (!result.Succeeded || result.Principal is null)
            {
                _logger.LogWarning("VK ID authentication failed");
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=vkid_auth_failed");
            }

            // Извлекаем данные
            string? email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            string? vkUserId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? phoneNumber = result.Principal.FindFirst(ClaimTypes.MobilePhone)?.Value;
            string? name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(vkUserId))
            {
                _logger.LogError("VK UserId is null");
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=missing_vk_id");
            }

            _logger.LogInformation("Processing VK ID login for email: {Email}, VKUserId: {VKUserId}", email, vkUserId);

            AuthResponseDto tokenResponse = await _twoFactorAuthEmailProcessor.ProcessExternalLoginAsync(
                provider: "VK ID",
                providerKey: vkUserId,
                email: email ?? "", // VK может не дать email, если пользователь скрыл
                name: name,
                phoneNumber: phoneNumber
            );

            if (tokenResponse.IsAuthSuccessful)
            {
                // ВАЖНО: VK может не дать email, нужно обработать этот кейс
                if (string.IsNullOrEmpty(email))
                {
                    // Если email нет - редиректим на страницу где пользователь введет email сам
                    return Redirect($"{Request.Scheme}://{Request.Host}/auth/complete-profile?token={tokenResponse.AccessToken}&provider=vkid");
                }

                return Redirect($"{Request.Scheme}://{Request.Host}/auth/vkid-callback?Token={tokenResponse.AccessToken}");
            }

            return Redirect($"{Request.Scheme}://{Request.Host}/login?error=vkid_auth_failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in VK ID callback: {ex.Message}");
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
}
