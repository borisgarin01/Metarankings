using IdentityLibrary.Models;
using System.IdentityModel.Tokens.Jwt;

namespace BlazorClient.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAuthService _authService;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger;

    public JwtAuthenticationStateProvider(
        IHttpClientFactory httpClientFactory,
        IAuthService authService,
        ILogger<JwtAuthenticationStateProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _authService = authService;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _logger.LogInformation("Getting authentication state");

        try
        {
            // Используем сервис для получения токена
            var accessToken = await _authService.GetCurrentAccessTokenAsync();

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("Access token missing, user not authenticated");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Проверяем, не истек ли токен
            if (IsTokenExpired(accessToken))
            {
                _logger.LogWarning("Token expired, attempting refresh");

                var refreshResult = await _authService.RefreshTokenAsync();

                if (refreshResult == null || !refreshResult.IsAuthSuccessful || string.IsNullOrEmpty(refreshResult.AccessToken))
                {
                    _logger.LogWarning("Token refresh failed, logging out");
                    await _authService.LogoutAsync();
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                accessToken = refreshResult.AccessToken;
                _logger.LogInformation("Token successfully refreshed");
            }

            // Создаем identity из токена
            var identity = GetClaimsIdentity(accessToken);

            if (!identity.IsAuthenticated)
            {
                _logger.LogWarning("Identity not authenticated, logging out");
                await _authService.LogoutAsync();
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var user = new ClaimsPrincipal(identity);
            _logger.LogInformation("User authenticated: {User}", user.Identity?.Name ?? "Unknown");

            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAuthenticationStateAsync");
            await _authService.LogoutAsync();
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    private bool IsTokenExpired(string token)
    {
        if (string.IsNullOrEmpty(token))
            return true;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(-1);
        }
        catch
        {
            return true;
        }
    }

    private ClaimsIdentity GetClaimsIdentity(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims;

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

            // Сохраняем токены через сервис
            await _authService.StoreAccessTokenAsync(model.AccessToken);
            await _authService.StoreRefreshTokenAsync(model.RefreshToken);
            _authService.AddDefaultRequestHeaderBearer(model.AccessToken);

            // Создаем identity и уведомляем об изменении состояния
            var identity = GetClaimsIdentity(model.AccessToken);

            if (!identity.IsAuthenticated)
            {
                _logger.LogError("Identity not authenticated after creation");
                throw new InvalidOperationException("Failed to create authenticated identity");
            }

            var user = new ClaimsPrincipal(identity);
            _logger.LogInformation("User successfully authenticated: {User}",
                user.Identity?.Name ?? "Unknown");

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
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
            await _authService.LogoutAsync();

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            _logger.LogInformation("User successfully logged out");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging out user");
            throw;
        }
    }
}