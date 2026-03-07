using Domain.Auth;
using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using IdentityLibrary.Telegram;
using MailKit.Net.Smtp;
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
    private readonly TelegramAuthenticator _telegramAuthenticator;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ILogger<AuthController> _logger;
    private readonly IOptionsMonitor<AuthSettings> _authSettingsOptionsMonitor;
    private readonly IOptionsMonitor<TokenValidationParameters> _tokenValidationParameters;
    private readonly IOptionsMonitor<EmailSettings> _emailSettings;
    public AuthController(IConfiguration configuration, UserManager<ApplicationUser> usersManager, TelegramAuthenticator telegramAuthenticator, IPasswordHasher<ApplicationUser> passwordHasher, ILogger<AuthController> logger, IOptionsMonitor<AuthSettings> authSettingsOptionsMonitor, IOptionsMonitor<EmailSettings> emailSettings, IOptionsMonitor<TokenValidationParameters> tokenValidationParameters, SignInManager<ApplicationUser> signInManager)
    {
        _configuration = configuration;
        _usersManager = usersManager;
        _telegramAuthenticator = telegramAuthenticator;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _authSettingsOptionsMonitor = authSettingsOptionsMonitor;
        _emailSettings = emailSettings;
        _tokenValidationParameters = tokenValidationParameters;
        _signInManager = signInManager;
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPost("addExternalLogin/{providerUserId}")]
    public async Task<ActionResult> AddExternalLogin(string providerUserId)
    {
        try
        {
            ApplicationUser? authUser = await _usersManager.FindByIdAsync(HttpContext.User.Claims.Single(b => b.Type == ClaimTypes.NameIdentifier).Value);
            if (authUser is null)
                return NotFound();

            IdentityResult identityResult = await _usersManager.AddLoginAsync(authUser, new UserLoginInfo("Google", providerUserId, "Google"));

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
    public async Task<ActionResult<IEnumerable<AuthenticationScheme>>> GetExternalProviders()
    {
        IEnumerable<Microsoft.AspNetCore.Authentication.AuthenticationScheme> externalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
        return Ok(externalProviders.Select(ep => new AuthenticationScheme(ep.Name, ep.DisplayName)));
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
            return Ok(new LoginResponse
            {
                RequiresTwoFactor = userToCheckExistance.TwoFactorEnabled,
                UserId = userToCheckExistance.Id.ToString(),
                Message = "2FA code sent to email"
            });
        }

        // If no 2FA, generate token immediately
        SymmetricSecurityKey secretKey = new(Encoding.UTF8.GetBytes(_authSettingsOptionsMonitor.CurrentValue.Secret));
        SigningCredentials signingCredentials = new(secretKey, SecurityAlgorithms.HmacSha512);

        List<Claim> userClaims = new();

        if (await _usersManager.IsInRoleAsync(userToCheckExistance, "Admin"))
            userClaims.Add(new Claim(ClaimTypes.Role, "Admin"));

        userClaims.Add(new Claim("EmailConfirmed", userToCheckExistance.EmailConfirmed.ToString()));
        userClaims.Add(new Claim("TwoFactorEnabled", userToCheckExistance.TwoFactorEnabled.ToString()));
        userClaims.Add(new Claim(ClaimTypes.NameIdentifier, userToCheckExistance.Id.ToString()));
        userClaims.Add(new Claim("UserName", userToCheckExistance.UserName));

        JwtSecurityToken tokenOptions = new(
            issuer: _authSettingsOptionsMonitor.CurrentValue.Issuer,
            audience: _authSettingsOptionsMonitor.CurrentValue.Audience,
            claims: userClaims,
            expires: DateTime.Now.AddHours(_authSettingsOptionsMonitor.CurrentValue.TokenLifetimeHours),
            signingCredentials: signingCredentials);

        userToCheckExistance.SecurityStamp = DateTime.Now.ToString();

        string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        IdentityResult identityResult = await _usersManager.SetAuthenticationTokenAsync(userToCheckExistance, "SQLServer", "AuthToken", tokenString);

        if (identityResult.Succeeded)
            return Ok(new { Token = tokenString });

        return StatusCode(500, "Authentication token setting has been failed");
    }

    [HttpPost("ConfirmLoginViaEmail")]
    public async Task<ActionResult> ConfirmLoginViaEmail(ConfirmLoginModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserId) || string.IsNullOrWhiteSpace(model.TwoFactorToken))
            return BadRequest("User ID and token are required");

        ApplicationUser? userToCheckExistance = await _usersManager.FindByIdAsync(model.UserId);

        if (userToCheckExistance is null)
            return NotFound();

        bool isValidTwoFactorToken = await _usersManager.VerifyTwoFactorTokenAsync(userToCheckExistance, "Email", model.TwoFactorToken);

        if (isValidTwoFactorToken)
        {
            SymmetricSecurityKey secretKey = new(Encoding.UTF8.GetBytes(_authSettingsOptionsMonitor.CurrentValue.Secret));
            SigningCredentials signingCredentials = new(secretKey, SecurityAlgorithms.HmacSha512);

            userToCheckExistance.SecurityStamp = DateTime.Now.ToString();

            List<Claim> userClaims = new();

            if (await _usersManager.IsInRoleAsync(userToCheckExistance, "Admin"))
                userClaims.Add(new Claim(ClaimTypes.Role, "Admin"));

            userClaims.Add(new Claim("EmailConfirmed", userToCheckExistance.EmailConfirmed.ToString()));
            userClaims.Add(new Claim("TwoFactorEnabled", userToCheckExistance.TwoFactorEnabled.ToString()));
            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, userToCheckExistance.Id.ToString()));
            userClaims.Add(new Claim("UserName", userToCheckExistance.UserName));

            JwtSecurityToken tokenOptions = new(
                issuer: _authSettingsOptionsMonitor.CurrentValue.Issuer,
                audience: _authSettingsOptionsMonitor.CurrentValue.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddHours(_authSettingsOptionsMonitor.CurrentValue.TokenLifetimeHours),
                signingCredentials: signingCredentials);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            IdentityResult identityResult = await _usersManager.SetAuthenticationTokenAsync(userToCheckExistance, "SQLServer", "AuthToken", tokenString);

            if (identityResult.Succeeded)
                return Ok(new { Token = tokenString });
        }

        return BadRequest("Invalid 2FA token");
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> Logout()
    {
        try
        {
            ApplicationUser authorizedApplicationUser = await _usersManager.FindByIdAsync(User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value);
            IdentityResult logoutResult = await _usersManager.RemoveAuthenticationTokenAsync(authorizedApplicationUser, "SQLServer", "AuthToken");
            if (logoutResult.Succeeded)
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
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
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
    public async Task<ActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
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
    public async Task<ActionResult> ResetPasswordConfirm(ResetPasswordConfirmModel resetPasswordModel)
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
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> SetTwoFactorEnabled(SetTwoFactorEnabledModel setTwoFactorEnabledModel)
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
}
