using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace BlazorClient.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger;
    private readonly IConfiguration _configuration;

    public JwtAuthenticationStateProvider(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        ILogger<JwtAuthenticationStateProvider> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _logger = logger;
        _configuration = configuration;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        return CreateAuthenticationState(token);
    }

    public void MarkUserAsAuthenticated(string token)
    {
        var authState = CreateAuthenticationState(token);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));

        // Set default authorization header
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    private AuthenticationState CreateAuthenticationState(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Create claims from the token without full validation for client-side
            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating authentication state from token");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}