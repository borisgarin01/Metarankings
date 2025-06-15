using API.Models.Identity;
using IdentityLibrary.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public sealed class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<IdentityResult>> Register(RegisterViewModel registerViewModel)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = registerViewModel.Email, Email = registerViewModel.Email };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            return Ok(result);
        }

        return BadRequest();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var claims = new Claim[]
                {
                    new Claim("Demo","Value")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookie");
                var claimPrincipal = new ClaimsPrincipal(claimsIdentity);

                return Ok(claimPrincipal.Claims.First().Value);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Unauthorized(model);
            }
        }

        return BadRequest(model);
    }

    [Authorize]
    [HttpGet]
    public ActionResult<string> GetLyrics()
    {
        return @"Однажды, в студёную зимнюю пору я из лесу вышел,
Был сильный мороз.
Гляжу, поднимается медленно в гору
Лошадка, везущая хворосту воз.";
    }
}
