using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _usersManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public AuthController(IConfiguration configuration, UserManager<ApplicationUser> usersManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _configuration = configuration;
        _usersManager = usersManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
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

        var tokenOptions = new JwtSecurityToken(issuer: _configuration["Auth:Issuer"], audience: _configuration["Auth:Audience"], new List<Claim>(), expires: DateTime.Now.AddHours(1), signingCredentials: signingCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return Ok(new { Token = tokenString });
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
        await _usersManager.CreateAsync(user);


        if (registerModel.UserEmail == "admin@admin.com")
        {
            await _usersManager.AddToRoleAsync(user, "Admin");
        }
        else
        {
            await _usersManager.AddToRoleAsync(user, "User");
        }

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth:Secret"]));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);

        var tokenOptions = new JwtSecurityToken(issuer: _configuration["Auth:Issuer"], audience: _configuration["Auth:Audience"], new List<Claim>(), expires: DateTime.Now.AddHours(1), signingCredentials: signingCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return Ok(new { Token = tokenString });
    }
}
