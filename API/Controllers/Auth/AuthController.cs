using IdentityLibrary.DTOs;
using IdentityLibrary.Models;

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


        if (string.IsNullOrWhiteSpace(loginModel.Email) || string.IsNullOrWhiteSpace(loginModel.Password))
            return BadRequest("Email и пароль должны быть указаны");

        var userToCheckExistance = await _usersManager.FindByEmailAsync(loginModel.Email);

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

        var tokenOptions = new JwtSecurityToken(issuer: _configuration["Auth:Issuer"], audience: _configuration["Auth:Audience"], userClaims, expires: DateTime.Now.AddHours(1), signingCredentials: signingCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return Ok(new
        {
            Token = tokenString
        });
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
        var identityResult = await _usersManager.CreateAsync(user);

        user = await _usersManager.FindByEmailAsync(user.Email);

        var userClaims = new List<Claim>();

        if (identityResult.Succeeded && string.Equals(registerModel.UserEmail, _configuration["Auth:AdminEmail"]) && string.Equals(registerModel.Password, _configuration["Auth:AdminPassword"]))
        {
            await _usersManager.AddToRoleAsync(user, "Admin");
            userClaims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        await _usersManager.AddToRoleAsync(user, "User");
        userClaims.Add(new Claim(ClaimTypes.Role, "User"));

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth:Secret"]));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);

        var tokenOptions = new JwtSecurityToken(issuer: _configuration["Auth:Issuer"], audience: _configuration["Auth:Audience"], userClaims, expires: DateTime.Now.AddHours(1), signingCredentials: signingCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return Ok(new { Token = tokenString });
    }
}
