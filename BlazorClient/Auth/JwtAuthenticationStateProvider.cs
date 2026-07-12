using IdentityLibrary.Models;
using System.IdentityModel.Tokens.Jwt;

namespace BlazorClient.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger;
    private readonly IAuthService _authService;

    public JwtAuthenticationStateProvider(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        ILogger<JwtAuthenticationStateProvider> logger,
        IAuthService authService)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _logger = logger;
        _authService = authService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            string? accessToken = await _localStorage.GetItemAsync<string>("accessToken");

            if (string.IsNullOrEmpty(accessToken))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            // Проверяем, не истек ли токен
            if (IsTokenExpired(accessToken))
            {
                // Пробуем обновить токен
                TokenResponse? refreshResult = await _authService.RefreshTokenAsync();

                if (refreshResult == null || !refreshResult.Success)
                {
                    await MarkUserAsLoggedOut();
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                accessToken = refreshResult.AccessToken;
            }

            ClaimsIdentity identity = GetClaimsIdentity(accessToken);
            ClaimsPrincipal user = new(identity);
            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAuthenticationStateAsync");
            await MarkUserAsLoggedOut();
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    private bool IsTokenExpired(string token)
    {
        if (string.IsNullOrEmpty(token))
            return true;

        try
        {
            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken? jwtToken = handler.ReadJwtToken(token);

            if (jwtToken == null)
                return true;

            return jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(-1); // Небольшой запас
        }
        catch
        {
            return true;
        }
    }

    private ClaimsIdentity GetClaimsIdentity(string token)
    {
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
        IEnumerable<Claim> claims = jwtToken.Claims;
        return new ClaimsIdentity(claims, "jwt");
    }

    public async Task MarkUserAsAuthenticated(LoginResponseModel model)
    {
        await _localStorage.SetItemAsync("accessToken", model.AccessToken);
        await _localStorage.SetItemAsync("refreshToken", model.RefreshToken);
        await _localStorage.SetItemAsync("sessionState", model);

        ClaimsIdentity identity = GetClaimsIdentity(model.AccessToken);
        ClaimsPrincipal user = new(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.RemoveItemAsync("sessionState");
        await _localStorage.RemoveItemAsync("accessToken");
        await _localStorage.RemoveItemAsync("refreshToken");
        ClaimsIdentity identity = new();
        ClaimsPrincipal user = new(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}