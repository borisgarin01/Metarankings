using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
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
    public AuthController(IConfiguration configuration, UserManager<ApplicationUser> usersManager)
    {
        _configuration = configuration;
        _usersManager = usersManager;
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

        var isCorrectPassword = BCrypt.Net.BCrypt.EnhancedVerify(loginModel.Password, userToCheckExistance.PasswordHash);

        if (!isCorrectPassword)
            return BadRequest("Неверный пароль");

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth:Secret"]));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);

        var userClaims = new List<Claim>();

        if (await _usersManager.IsInRoleAsync(userToCheckExistance, "Admin"))
            userClaims.Add(new Claim(ClaimTypes.Role, "Admin"));

        userClaims.Add(new Claim("EmailConfirmed", userToCheckExistance.EmailConfirmed.ToString()));

        var tokenOptions = new JwtSecurityToken(issuer: _configuration["Auth:Issuer"], audience: _configuration["Auth:Audience"], userClaims, expires: DateTime.Now.AddHours(1), signingCredentials: signingCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return Ok(tokenString);
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(RegisterModel registerModel)
    {
        var userToCheckExistance = await _usersManager.FindByEmailAsync(registerModel.UserEmail);

        if (userToCheckExistance is not null)
            return BadRequest($"Пользователь с {registerModel.UserEmail} уже существует");

        if (!string.Equals(registerModel.Password, registerModel.PasswordConfirmation))
            return BadRequest("Пароль не совпадает с подтверждением пароля");

        var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(registerModel.Password);

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
            UserName = registerModel.UserName
        };

        IdentityResult userCreationResult = await _usersManager.CreateAsync(user);

        if (userCreationResult.Succeeded)
        {
            user = await _usersManager.FindByEmailAsync(registerModel.UserEmail);

            string code = WebUtility.UrlEncode(await _usersManager.GenerateEmailConfirmationTokenAsync(user));
            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Metarankings.ru", "boris.garin01job@mail.ru"));
            emailMessage.To.Add(new MailboxAddress("", user.Email));
            emailMessage.Subject = "Confirm email";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"Confirm email: go to email confirmation <a href=\"{callbackUrl}\">link</a> to confirm your email"
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru", 465, true);
                await client.AuthenticateAsync("boris.garin01job@mail.ru", "tWySJBDnb2Lfgirt1XSo");
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
}
