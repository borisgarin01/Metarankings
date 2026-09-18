using API.Auth;
using IdentityLibrary.DTOs;
using Microsoft.Extensions.Logging;
using Settings;
using System.Net.Http;
using Telegram.Bot.Requests.Abstractions;

namespace IdentityLibrary.Services.Classes;

public sealed class AuthTokenGenerator : IAuthTokenGenerator
{
    private readonly UserManager<ApplicationUser> _usersManager;
    private readonly IOptionsMonitor<AuthSettings> _authSettingsOptionsMonitor;
    private ILogger<AuthTokenGenerator> _logger;

    public AuthTokenGenerator(UserManager<ApplicationUser> usersManager, IOptionsMonitor<AuthSettings> authSettingsOptionsMonitor, ILogger<AuthTokenGenerator> logger)
    {
        _usersManager = usersManager;
        _authSettingsOptionsMonitor = authSettingsOptionsMonitor;
        _logger = logger;
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

    public string GenerateCodeVerifier()
    {
        // 43-128 символов, base64url
        byte[] bytes = new byte[32];
        System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public string GenerateCodeChallenge(string verifier)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        byte[] challengeBytes = sha256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(verifier));
        return Convert.ToBase64String(challengeBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public async Task<string?> ExchangeVkCodeForUserIdAsync(string code, string codeVerifier, string scheme, string host)
    {
        using var http = new HttpClient();

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["client_id"] = _authSettingsOptionsMonitor.CurrentValue.Vk.ClientId,
            ["code_verifier"] = codeVerifier,
            ["redirect_uri"] = $"{scheme}://{host}/api/auth/vkid-link-callback"
        });

        HttpResponseMessage response = await http.PostAsync("https://id.vk.ru/oauth2/auth", content);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("VK token exchange failed: {Status}", response.StatusCode);
            return null;
        }

        string json = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("VK token exchange response: {Json}", json);

        using var doc = System.Text.Json.JsonDocument.Parse(json);

        if (doc.RootElement.TryGetProperty("user_id", out var userIdProp))
        {
            return userIdProp.ValueKind == System.Text.Json.JsonValueKind.String
                ? userIdProp.GetString()
                : userIdProp.GetInt64().ToString();
        }

        return null;
    }
}
