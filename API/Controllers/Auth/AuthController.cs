using Domain.Auth;
using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using IdentityLibrary.Telegram;
using MailKit.Net.Smtp;
using MimeKit;
using System.Net;

namespace API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _usersManager;
    private readonly TelegramAuthenticator _telegramAuthenticator;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ILogger<AuthController> _logger;
    public AuthController(IConfiguration configuration, UserManager<ApplicationUser> usersManager, TelegramAuthenticator telegramAuthenticator, IPasswordHasher<ApplicationUser> passwordHasher, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _usersManager = usersManager;
        _telegramAuthenticator = telegramAuthenticator;
        _passwordHasher = passwordHasher;
        _logger = logger;

    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginModel loginModel)
    {
        if (loginModel is null)
            return BadRequest("Неверный логин");


        if (string.IsNullOrWhiteSpace(loginModel.UserEmail) || string.IsNullOrWhiteSpace(loginModel.Password))
            return BadRequest("Email и пароль должны быть указаны");

        var userToCheckExistance = await _usersManager.FindByEmailAsync(loginModel.UserEmail);

        if (userToCheckExistance is null)
            return NotFound("Пользователь не зарегистрирован");

        PasswordVerificationResult passwordVerificationResult = _passwordHasher.VerifyHashedPassword(userToCheckExistance, userToCheckExistance.PasswordHash, loginModel.Password);

        if (passwordVerificationResult != PasswordVerificationResult.Success)
            return BadRequest("Неверный пароль");

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth:Secret"]));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);

        var userClaims = new List<Claim>();

        if (await _usersManager.IsInRoleAsync(userToCheckExistance, "Admin"))
            userClaims.Add(new Claim(ClaimTypes.Role, "Admin"));

        userClaims.Add(new Claim("EmailConfirmed", userToCheckExistance.EmailConfirmed.ToString()));

        userClaims.Add(new Claim(ClaimTypes.NameIdentifier, userToCheckExistance.Id.ToString()));

        var tokenOptions = new JwtSecurityToken(issuer: _configuration["Auth:Issuer"], audience: _configuration["Auth:Audience"], userClaims, expires: DateTime.Now.AddHours(1), signingCredentials: signingCredentials);

        userToCheckExistance.SecurityStamp = DateTime.Now.ToString();

        string twoFactorToken = await _usersManager.GenerateTwoFactorTokenAsync(userToCheckExistance, "Email");

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        IdentityResult identityResult = await _usersManager.SetAuthenticationTokenAsync(userToCheckExistance, "SQLServer", "AuthToken", tokenString);

        if (identityResult.Succeeded)
            return Ok(tokenString);

        return StatusCode(500, "Authentication token setting has been failed");
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
        var userToCheckExistance = await _usersManager.FindByEmailAsync(registerModel.UserEmail);

        if (userToCheckExistance is not null)
            return BadRequest($"Пользователь с {registerModel.UserEmail} уже существует");
        userToCheckExistance = registeredUsers.FirstOrDefault(b => b.NormalizedUserName == registerModel.UserName.ToUpperInvariant());
        if (userToCheckExistance is not null)
            return BadRequest($"Пользователь с логином {registerModel.UserName} уже существует");

        if (!string.Equals(registerModel.Password, registerModel.PasswordConfirmation))
            return BadRequest("Пароль не совпадает с подтверждением пароля");

        var passwordHash = _passwordHasher.HashPassword(null, registerModel.Password);

        var user = new ApplicationUser
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
            user = await _usersManager.FindByEmailAsync(registerModel.UserEmail);

            string code = WebUtility.UrlEncode(await _usersManager.GenerateEmailConfirmationTokenAsync(user));
            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_configuration["EmailSettings:Sender:Name"], _configuration["EmailSettings:Sender:Email"]));
            emailMessage.To.Add(new MailboxAddress("", user.Email));
            emailMessage.Subject = "Confirm email";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"Confirm email: go to email confirmation <a href=\"{callbackUrl}\">link</a> to confirm your email"
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["EmailSettings:Host"], int.Parse(_configuration["EmailSettings:Port"]), bool.Parse(_configuration["EmailSettings:UseSsl"]));
                await client.AuthenticateAsync(_configuration["EmailSettings:UserName"], _configuration["EmailSettings:Password"]);
                await client.SendAsync(emailMessage);

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

        var user = await _usersManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound("User not found");

        var emailConfirmationResult = await _usersManager.ConfirmEmailAsync(user, code);

        if (!emailConfirmationResult.Succeeded)
            return StatusCode(StatusCodes.Status400BadRequest, userId);

        return Ok($"Email {user.Email} подтверждён.");
    }

    [HttpPost("assignToAdmin")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult> AssignToAdmin(string humanToAssignToAdminEmail)
    {
        var humanToAssignToAdmin = await _usersManager.FindByEmailAsync(humanToAssignToAdminEmail);

        if (humanToAssignToAdmin is null)
            return NotFound("Human to assign to admin not found");

        else
        {
            IdentityResult identityResult = await _usersManager.AddToRoleAsync(humanToAssignToAdmin, "Admin");
            if (identityResult is null)
                return NotFound();
            if (!identityResult.Succeeded)
                return BadRequest(identityResult);
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

        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress(_configuration["EmailSettings:Sender:Name"], _configuration["EmailSettings:Sender:Email"]));
        emailMessage.To.Add(new MailboxAddress("", user.Email));
        emailMessage.Subject = "Reset password";
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $"Reset password token - {resetPasswordToken}"
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_configuration["EmailSettings:Host"], int.Parse(_configuration["EmailSettings:Port"]), bool.Parse(_configuration["EmailSettings:UseSsl"]));
            await client.AuthenticateAsync(_configuration["EmailSettings:UserName"], _configuration["EmailSettings:Password"]);
            await client.SendAsync(emailMessage);

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
        return BadRequest();
    }
}
