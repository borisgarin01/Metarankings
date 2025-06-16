using API.Models.Identity;
using BCrypt.Net;
using IdentityLibrary.DTOs;
using IdentityLibrary.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly JwtProvider _jwtProvider;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        JwtProvider jwtProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtProvider = jwtProvider;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToArray();
        return Ok(users);
    }

    [HttpPost("Register")]
    public async Task<ActionResult> AddApplicationUserAsync(RegisterViewModel registerViewModel)
    {
        if (await _userManager.FindByEmailAsync(registerViewModel.Email) is not null)
            return BadRequest("Duplicate email");

        var applicationUser = new ApplicationUser
        {
            Email = registerViewModel.Email,
            EmailConfirmed = false,
            NormalizedEmail = registerViewModel.Email.ToUpper(),
            NormalizedUserName = registerViewModel.Email.ToUpper(),
            PhoneNumber = registerViewModel.Phone,
            UserName = registerViewModel.Email,
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(registerViewModel.Password),
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false
        };

        await _userManager.CreateAsync(applicationUser);
        return Ok(applicationUser);
    }

    [HttpPost("Login")]
    public async Task<ActionResult> LoginAsync(LoginViewModel loginViewModel)
    {
        var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

        if (user is null)
            return NotFound("User not found");

        var passwordVerificationResult = BCrypt.Net.BCrypt.EnhancedVerify(loginViewModel.Password, user.PasswordHash);


        if (passwordVerificationResult is false)
        {
            return BadRequest("Failed to login");
        }
        var token = _jwtProvider.GenerateToken(user);

        return Ok(token);
    }
}
