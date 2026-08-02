using API.Auth;
using IdentityLibrary.DTOs;
using Settings;

namespace IdentityLibrary.Services.Classes;

public sealed class AuthTokenGenerator : IAuthTokenGenerator
{
    private readonly UserManager<ApplicationUser> _usersManager;
    private readonly IOptionsMonitor<AuthSettings> _authSettingsOptionsMonitor;

    public AuthTokenGenerator(UserManager<ApplicationUser> usersManager, IOptionsMonitor<AuthSettings> authSettingsOptionsMonitor)
    {
        _usersManager = usersManager;
        _authSettingsOptionsMonitor = authSettingsOptionsMonitor;
    }

    public async Task<string> GenerateAccessToken(ApplicationUser user)
    {
        SymmetricSecurityKey secretKey = new(Encoding.UTF8.GetBytes(_authSettingsOptionsMonitor.CurrentValue.AccessSecret));
        SigningCredentials signingCredentials = new(secretKey, SecurityAlgorithms.HmacSha512);

        List<Claim> userClaims = new()
    {
        new Claim("EmailConfirmed", user.EmailConfirmed.ToString()),
        new Claim("TwoFactorEnabled", user.TwoFactorEnabled.ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim("UserName", user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

        if (await _usersManager.IsInRoleAsync(user, "Admin"))
            userClaims.Add(new Claim(ClaimTypes.Role, "Admin"));

        JwtSecurityToken tokenOptions = new(
            issuer: _authSettingsOptionsMonitor.CurrentValue.Issuer,
            audience: _authSettingsOptionsMonitor.CurrentValue.Audience,
            claims: userClaims,
            expires: DateTime.UtcNow.AddMinutes(_authSettingsOptionsMonitor.CurrentValue.AccessTokenLifetimeMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
