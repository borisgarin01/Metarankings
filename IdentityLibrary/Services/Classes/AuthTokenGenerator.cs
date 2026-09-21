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

    public async Task<string?> ExchangeVkCodeForUserIdAsync(string code, string codeVerifier, string scheme, string host, string deviceId)
    {
        using var http = new HttpClient();

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["client_id"] = _authSettingsOptionsMonitor.CurrentValue.VkId.ClientId,
            ["code_verifier"] = codeVerifier,
            ["device_id"] = deviceId,  // ← добавить
            ["redirect_uri"] = $"{scheme}://{host}/signin-vkid"
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
    public async Task<string?> ExchangeYandexCodeForUserIdAsync(string code, string codeVerifier, string scheme, string host, string deviceId)
    {
        using var http = new HttpClient();

        // Yandex token endpoint
        var tokenUrl = "https://oauth.yandex.ru/token";

        // Build the request content
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["client_id"] = _authSettingsOptionsMonitor.CurrentValue.YandexId.ClientId,
            ["code_verifier"] = codeVerifier,
            // Ensure this matches exactly what is registered in your Yandex App settings
            // and what is used to generate the initial auth URL in the controller.
            ["redirect_uri"] = $"{scheme}://{host}/api/auth/yandexid-link-callback"
        });

        // Yandex uses Basic Auth for confidential clients.
        // This sends 'client_id:client_secret' in the Authorization header.
        var clientSecret = _authSettingsOptionsMonitor.CurrentValue.YandexId.ClientSecret;
        var authHeader = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{_authSettingsOptionsMonitor.CurrentValue.YandexId.ClientId}:{clientSecret}"));
        http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

        HttpResponseMessage response = await http.PostAsync(tokenUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("Yandex token exchange failed: {Status}, Body: {Body}", response.StatusCode, errorBody);
            return null;
        }

        string json = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Yandex token exchange response: {Json}", json);

        using var doc = System.Text.Json.JsonDocument.Parse(json);

        // The token response usually contains 'access_token'.
        // We need to use it to get the user's unique ID.
        if (doc.RootElement.TryGetProperty("access_token", out var accessTokenProp))
        {
            var accessToken = accessTokenProp.GetString();

            // Now call the user info endpoint
            using var userInfoHttp = new HttpClient();
            userInfoHttp.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", accessToken);

            var userInfoResponse = await userInfoHttp.GetAsync("https://login.yandex.ru/info?format=json");
            if (!userInfoResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Yandex user info request failed: {Status}", userInfoResponse.StatusCode);
                return null;
            }

            var userJson = await userInfoResponse.Content.ReadAsStringAsync();
            using var userDoc = System.Text.Json.JsonDocument.Parse(userJson);

            // The unique user identifier is in the 'id' field.
            if (userDoc.RootElement.TryGetProperty("id", out var userIdProp))
            {
                return userIdProp.GetString();
            }
        }

        return null;
    }
}
