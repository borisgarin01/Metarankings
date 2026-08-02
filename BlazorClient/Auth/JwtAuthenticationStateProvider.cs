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
        _logger.LogInformation("Getting authentication state");

        try
        {
            string? accessToken = await _localStorage.GetItemAsync<string>("accessToken");
            _logger.LogDebug("Access token from localStorage: {TokenStatus}",
                string.IsNullOrEmpty(accessToken) ? "MISSING" : "present");

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("Access token missing, user not authenticated");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _logger.LogDebug("Access token: {Token}", accessToken);

            bool isExpired = IsTokenExpired(accessToken);
            _logger.LogInformation("Token expiration check: {IsExpired}", isExpired);

            if (isExpired)
            {
                _logger.LogWarning("Token expired, attempting refresh");

                AuthResponseDto? refreshResult = await _authService.RefreshTokenAsync();
                _logger.LogDebug("Token refresh result: Success = {Success}",
                    refreshResult?.IsAuthSuccessful ?? false);

                if (refreshResult == null || !refreshResult.IsAuthSuccessful || string.IsNullOrEmpty(refreshResult.AccessToken))
                {
                    _logger.LogWarning("Token refresh failed or token empty, logging out");
                    await MarkUserAsLoggedOut();
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                accessToken = refreshResult.AccessToken;
                _logger.LogInformation("Token successfully refreshed, new access token obtained");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }

            ClaimsIdentity identity = GetClaimsIdentity(accessToken);

            if (!identity.IsAuthenticated)
            {
                _logger.LogWarning("Identity not authenticated, logging out");
                await MarkUserAsLoggedOut();
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            ClaimsPrincipal user = new(identity);

            _logger.LogInformation("User authenticated: {User}",
                user.Identity?.Name ?? "Unknown");

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
        _logger.LogDebug("Checking token expiration");

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Token empty during expiration check");
            return true;
        }

        try
        {
            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken? jwtToken = handler.ReadJwtToken(token);

            if (jwtToken == null)
            {
                _logger.LogWarning("Failed to read JWT token");
                return true;
            }

            DateTime expirationTime = jwtToken.ValidTo;
            DateTime now = DateTime.UtcNow;
            bool isExpired = expirationTime <= now.AddMinutes(-1);

            _logger.LogDebug("Token expiration: {Expiration}, Current time: {Now}, Expired: {IsExpired}",
                expirationTime, now, isExpired);

            if (isExpired)
            {
                var timeLeft = now - expirationTime;
                _logger.LogWarning("Token expired {TimeLeft} ago", timeLeft);
            }
            else
            {
                var timeLeft = expirationTime - now;
                _logger.LogDebug("Token valid, time left: {TimeLeft}", timeLeft);
            }

            return isExpired;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking token expiration");
            return true;
        }
    }

    private ClaimsIdentity GetClaimsIdentity(string token)
    {
        _logger.LogDebug("Extracting claims from token");

        try
        {
            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
            IEnumerable<Claim> claims = jwtToken.Claims;

            int claimCount = claims.Count();
            _logger.LogDebug("Extracted {Count} claims", claimCount);

            if (claimCount > 0 && _logger.IsEnabled(LogLevel.Trace))
            {
                var claimNames = string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}"));
                _logger.LogTrace("Claims: {Claims}", claimNames);
            }

            return new ClaimsIdentity(claims, "jwt");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting claims from token");
            return new ClaimsIdentity();
        }
    }

    public async Task MarkUserAsAuthenticated(LoginResponseModel model)
    {
        _logger.LogInformation("Marking user as authenticated");

        try
        {
            if (string.IsNullOrEmpty(model.AccessToken))
            {
                _logger.LogError("Access token empty during authentication");
                throw new InvalidOperationException("Access token cannot be null or empty");
            }

            _logger.LogDebug("Saving tokens to localStorage");
            await _localStorage.SetItemAsync("accessToken", model.AccessToken);
            await _localStorage.SetItemAsync("refreshToken", model.RefreshToken);
            await _localStorage.SetItemAsync("sessionState", model);

            _logger.LogDebug("Access token length: {AccessLength}, Refresh token length: {RefreshLength}",
                model.AccessToken?.Length ?? 0, model.RefreshToken?.Length ?? 0);

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", model.AccessToken);

            ClaimsIdentity identity = GetClaimsIdentity(model.AccessToken);

            if (!identity.IsAuthenticated)
            {
                _logger.LogError("Identity not authenticated after creation");
                throw new InvalidOperationException("Failed to create authenticated identity");
            }

            ClaimsPrincipal user = new(identity);

            _logger.LogInformation("User successfully authenticated: {User}",
                user.Identity?.Name ?? "Unknown");

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            _logger.LogDebug("Authentication state change notification sent");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking user as authenticated");
            throw;
        }
    }

    public async Task MarkUserAsLoggedOut()
    {
        _logger.LogInformation("Marking user as logged out");

        try
        {
            var accessToken = await _localStorage.GetItemAsync<string>("accessToken");
            var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

            _logger.LogDebug("Current tokens: Access = {AccessStatus}, Refresh = {RefreshStatus}",
                string.IsNullOrEmpty(accessToken) ? "MISSING" : "present",
                string.IsNullOrEmpty(refreshToken) ? "MISSING" : "present");

            await _localStorage.RemoveItemAsync("sessionState");
            await _localStorage.RemoveItemAsync("accessToken");
            await _localStorage.RemoveItemAsync("refreshToken");

            _httpClient.DefaultRequestHeaders.Authorization = null;

            _logger.LogDebug("Tokens removed from localStorage and headers");

            ClaimsIdentity identity = new();
            ClaimsPrincipal user = new(identity);

            _logger.LogInformation("User successfully logged out");

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            _logger.LogDebug("Authentication state change notification sent (logout)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging out user");
            throw;
        }
    }
}