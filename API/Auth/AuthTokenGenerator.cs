using IdentityLibrary.DTOs;
using Microsoft.Extensions.Options;
using Settings;

namespace API.Auth;

public sealed class AuthTokenGenerator
{
    private readonly UserManager<ApplicationUser> _usersManager;
    private readonly IOptionsMonitor<AuthSettings> _authSettingsOptionsMonitor;

    public AuthTokenGenerator(UserManager<ApplicationUser> usersManager, IOptionsMonitor<AuthSettings> authSettingsOptionsMonitor)
    {
        _usersManager = usersManager;
        _authSettingsOptionsMonitor = authSettingsOptionsMonitor;
    }

    public async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        SymmetricSecurityKey secretKey = new(Encoding.UTF8.GetBytes(_authSettingsOptionsMonitor.CurrentValue.Secret));
        SigningCredentials signingCredentials = new(secretKey, SecurityAlgorithms.HmacSha512);

        user.SecurityStamp = DateTime.Now.ToString();

        List<Claim> userClaims = new()
    {
        new Claim("EmailConfirmed", user.EmailConfirmed.ToString()),
        new Claim("TwoFactorEnabled", user.TwoFactorEnabled.ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim("UserName", user.UserName)
    };

        if (await _usersManager.IsInRoleAsync(user, "Admin"))
            userClaims.Add(new Claim(ClaimTypes.Role, "Admin"));

        JwtSecurityToken tokenOptions = new(
            issuer: _authSettingsOptionsMonitor.CurrentValue.Issuer,
            audience: _authSettingsOptionsMonitor.CurrentValue.Audience,
            claims: userClaims,
            expires: DateTime.Now.AddHours(_authSettingsOptionsMonitor.CurrentValue.TokenLifetimeHours),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}
