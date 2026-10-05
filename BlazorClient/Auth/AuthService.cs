using Blazored.Toast.Services;
using Domain.Auth;
using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using System.Net;
using System.Text;
using System.Threading;

namespace BlazorClient.Auth;

public class AuthService : IAuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<AuthService> _logger;
    private readonly IToastService _toastService;
    private readonly IHttpClientFactory _httpClientFactory;

    // Static: IHttpClientFactory resolves JwtAuthorizationHandler (and its IAuthService) in a
    // separate DI scope, so per-instance state would not be shared with the rest of the app.
    private static string? _cachedAccessToken;
    private static readonly SemaphoreSlim _refreshLock = new(1, 1);

    public AuthService(IHttpClientFactory httpClientFactory,
                      ILocalStorageService localStorage,
                      ILogger<AuthService> logger,
                      IToastService toastService)
    {
        _httpClientFactory = httpClientFactory;
        _localStorage = localStorage;
        _logger = logger;
        _toastService = toastService;
    }

    private const string ACCESS_KEY = nameof(ACCESS_KEY);
    private const string REFRESH_KEY = nameof(REFRESH_KEY);

    public async Task<LoginResponseModel> LoginAsync(LoginModel loginModel)
    {
        _logger.LogInformation("Login attempt for user: {Email}", loginModel.UserEmail);

        try
        {
            // ВАЖНО: Используем UnauthorizedClient для логина!
            var response = await _httpClientFactory.CreateClient("UnauthorizedClient")
                .PostAsJsonAsync("/api/auth/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponseModel>();
                _logger.LogInformation("Login successful for {Email}, TwoFactor: {TwoFactor}",
                    loginModel.UserEmail, result?.RequiresTwoFactor);

                // Если токены получены сразу (без 2FA) - сохраняем
                if (result != null && !string.IsNullOrEmpty(result.AccessToken))
                {
                    await StoreAccessTokenAsync(result.AccessToken);
                    await StoreRefreshTokenAsync(result.RefreshToken);
                    AddDefaultRequestHeaderBearer(result.AccessToken);
                }

                return result;
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Login failed for {Email}. Status: {Status}, Error: {Error}",
                loginModel.UserEmail, response.StatusCode, error);
            throw new Exception(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during login for {Email}", loginModel.UserEmail);
            throw;
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync()
    {
        _logger.LogInformation("Refreshing token");

        // Refresh tokens are single-use (rotated on the server), so concurrent refreshes with the
        // same token make all but the first fail with "Invalid refresh token". Serialize them and
        // reuse the result if another caller already refreshed while we were waiting.
        string? refreshTokenBeforeWait = await _localStorage.GetItemAsync<string>(REFRESH_KEY);
        await _refreshLock.WaitAsync();

        try
        {
            var refreshToken = await _localStorage.GetItemAsync<string>(REFRESH_KEY);

            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token missing");
                await LogoutAsync();
                return new AuthResponseDto(false, false, "Refresh token is missing", null, null);
            }

            if (refreshToken != refreshTokenBeforeWait)
            {
                _logger.LogInformation("Token was already refreshed by another request");
                return await GetStoredTokensResultAsync(refreshToken);
            }

            // ВАЖНО: Используем UnauthorizedClient для refresh-token!
            var client = _httpClientFactory.CreateClient("UnauthorizedClient");

            var request = new RefreshTokenRequest { RefreshToken = refreshToken };
            var response = await client.PostAsJsonAsync("/api/auth/refresh-token", request);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                if (tokenResponse != null && tokenResponse.IsAuthSuccessful)
                {
                    _logger.LogInformation("Token refreshed successfully");

                    // Сохраняем новые токены
                    await StoreAccessTokenAsync(tokenResponse.AccessToken);
                    await StoreRefreshTokenAsync(tokenResponse.RefreshToken);
                    AddDefaultRequestHeaderBearer(tokenResponse.AccessToken);

                    return tokenResponse;
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Refresh token failed: {Status}, {Error}", response.StatusCode, error);

                // Server unavailable (e.g. during a redeploy) - keep the tokens and retry later.
                if ((int)response.StatusCode >= 500)
                    return new AuthResponseDto(false, false, "Server unavailable", null, null);

                // Another browser tab may be rotating the same token right now (tabs share localStorage,
                // but not _refreshLock). Its response may not have been saved yet, so wait for it briefly.
                var rotatedByOtherTab = await WaitForTokenRotatedByOtherTabAsync(refreshToken);
                if (rotatedByOtherTab != null)
                {
                    _logger.LogInformation("Token was refreshed by another tab");
                    return await GetStoredTokensResultAsync(rotatedByOtherTab);
                }
            }

            // Clear only the local session. Calling /api/auth/logout here would revoke ALL of the
            // user's refresh tokens on the server, including the one another tab has just received.
            _logger.LogWarning("Token refresh failed");
            await ClearLocalTokensAsync();
            return new AuthResponseDto(false, false, "Failed to refresh token", null, null);
        }
        catch (Exception ex)
        {
            // Network error - don't drop the session, the refresh token may still be valid.
            _logger.LogError(ex, "Error refreshing token");
            return new AuthResponseDto(false, false, $"Error refreshing token: {ex.Message}", null, null);
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private async Task<string?> WaitForTokenRotatedByOtherTabAsync(string usedRefreshToken)
    {
        const int attempts = 10;
        const int delayMs = 300;

        for (int i = 0; i < attempts; i++)
        {
            var current = await _localStorage.GetItemAsync<string>(REFRESH_KEY);
            if (string.IsNullOrEmpty(current))
                return null; // другая вкладка разлогинилась
            if (current != usedRefreshToken)
                return current;

            await Task.Delay(delayMs);
        }

        return null;
    }

    private async Task ClearLocalTokensAsync()
    {
        try
        {
            await _localStorage.RemoveItemAsync(ACCESS_KEY);
            await _localStorage.RemoveItemAsync(REFRESH_KEY);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing local tokens");
        }

        _cachedAccessToken = null;
        _httpClientFactory.CreateClient("AuthorizedClient").DefaultRequestHeaders.Remove("Authorization");
    }

    private async Task<AuthResponseDto> GetStoredTokensResultAsync(string refreshToken)
    {
        var accessToken = await _localStorage.GetItemAsync<string>(ACCESS_KEY);
        _cachedAccessToken = accessToken;
        AddDefaultRequestHeaderBearer(accessToken);
        return new AuthResponseDto(true, false, string.Empty, accessToken, refreshToken);
    }

    public async Task<AuthResponseDto> VerifyTwoFactorAsync(string userId, string token)
    {
        _logger.LogInformation("Verifying 2FA for user: {UserId}", userId);

        try
        {
            ConfirmLoginModel request = new(userId, token);
            _logger.LogDebug("Sending 2FA code for {UserId}", userId);

            HttpResponseMessage response = await _httpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/auth/ConfirmLoginViaEmail", request);
            _logger.LogDebug("2FA response: StatusCode = {StatusCode}", response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Verification response: {Content}", responseContent);

                AuthResponseDto? result = JsonSerializer.Deserialize<AuthResponseDto>(responseContent);

                if (result != null && result.IsAuthSuccessful)
                {
                    _logger.LogInformation("2FA verification successful for {UserId}", userId);

                    await StoreAccessTokenAsync(result.AccessToken);
                    await StoreRefreshTokenAsync(result.RefreshToken);

                    AddDefaultRequestHeaderBearer(result.AccessToken);

                    return result;
                }
                else
                {
                    _logger.LogWarning("2FA verification returned Success=false for {UserId}", userId);
                }
            }

            string error = await response.Content.ReadAsStringAsync();
            _logger.LogError("2FA verification failed for {UserId}. Status: {Status}, Error: {Error}",
                userId, response.StatusCode, error);
            return new AuthResponseDto(false, false, null, null, "Verification error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during 2FA verification for {UserId}", userId);
            return new AuthResponseDto(false, false, null, null, $"Verification error: {ex.Message}");
        }
    }

    public async Task StoreAccessTokenAsync(string token)
    {
        _logger.LogInformation("Saving access token");

        try
        {
            await _localStorage.SetItemAsync(ACCESS_KEY, token);
            _cachedAccessToken = token; // Обновляем кеш
            AddDefaultRequestHeaderBearer(token);
            _logger.LogDebug("Access token saved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving access token");
            throw;
        }
    }

    public async Task StoreRefreshTokenAsync(string refreshToken)
    {
        _logger.LogInformation("Saving refresh token");

        try
        {
            await _localStorage.SetItemAsync(REFRESH_KEY, refreshToken);
            _logger.LogDebug("Refresh token saved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving refresh token");
            throw;
        }
    }

    public async Task LogoutAsync()
    {
        _logger.LogInformation("Starting logout");

        try
        {
            // Пытаемся отозвать refresh token на сервере. Не требует валидного access token:
            // сервер принимает сам refresh token как доказательство.
            string? refreshToken = await _localStorage.GetItemAsync<string>(REFRESH_KEY);
            string? accessToken = await _localStorage.GetItemAsync<string>(ACCESS_KEY);

            if (!string.IsNullOrEmpty(refreshToken) || !string.IsNullOrEmpty(accessToken))
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout")
                {
                    Content = JsonContent.Create(new RefreshTokenRequest { RefreshToken = refreshToken ?? string.Empty })
                };
                if (!string.IsNullOrEmpty(accessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClientFactory.CreateClient("UnauthorizedClient").SendAsync(request, cts.Token);
                if (!response.IsSuccessStatusCode)
                    _logger.LogWarning("Server logout returned {Status}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            // Сервер недоступен / таймаут / ошибка — всё равно разлогиниваемся локально
            _logger.LogWarning(ex, "Server logout failed, logging out locally");
        }
        finally
        {
            await ClearLocalTokensAsync();
            _logger.LogInformation("Logout completed");
        }
    }

    public async Task RegisterAsync(RegisterModel registerModel)
    {
        _logger.LogInformation("Registering user: {Email}", registerModel.UserEmail);
        _logger.LogDebug("Registration data: {RegisterData}", JsonSerializer.Serialize(registerModel));

        try
        {
            HttpResponseMessage httpResponseMessage = await _httpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/auth/register", registerModel);
            _logger.LogDebug("Registration response: StatusCode = {StatusCode}", httpResponseMessage.StatusCode);

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest ||
                httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                string error = await httpResponseMessage.Content.ReadAsStringAsync();
                _logger.LogError("Registration failed for {Email}. Status: {Status}, Error: {Error}",
                    registerModel.UserEmail, httpResponseMessage.StatusCode, error);
                throw new Exception(error);
            }

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("Registration successful for {Email}", registerModel.UserEmail);
            }
            else
            {
                _logger.LogWarning("Registration returned unexpected status: {Status}",
                    httpResponseMessage.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during registration for {Email}", registerModel.UserEmail);
            throw;
        }
    }

    public async Task<HttpResponseMessage> SendResetPasswordConfirmMessage(ResetPasswordConfirmModel resetPasswordConfirmModel)
    {
        _logger.LogInformation("Sending password reset confirmation for: {Email}",
            resetPasswordConfirmModel.Email);

        try
        {
            HttpResponseMessage httpResponseMessage = await _httpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync(
                "/api/auth/resetPasswordConfirm", resetPasswordConfirmModel);

            _logger.LogDebug("Password reset confirmation response: {StatusCode}",
                httpResponseMessage.StatusCode);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("Password reset confirmation sent for {Email}",
                    resetPasswordConfirmModel.Email);
            }
            else
            {
                string error = await httpResponseMessage.Content.ReadAsStringAsync();
                _logger.LogWarning("Password reset confirmation error: {Status}, Error: {Error}",
                    httpResponseMessage.StatusCode, error);
            }

            return httpResponseMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception sending password reset confirmation");
            throw;
        }
    }

    public async Task<HttpResponseMessage> SendResetPasswordMessage(ResetPasswordModel resetPasswordModel)
    {
        _logger.LogInformation("Sending password reset request for: {Email}",
            resetPasswordModel.Email);

        try
        {
            HttpResponseMessage httpResponseMessage = await _httpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync<ResetPasswordModel>(
                "/api/auth/resetPassword", resetPasswordModel);

            _logger.LogDebug("Password reset response: {StatusCode}", httpResponseMessage.StatusCode);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("Password reset request sent for {Email}",
                    resetPasswordModel.Email);
            }
            else
            {
                string error = await httpResponseMessage.Content.ReadAsStringAsync();
                _logger.LogWarning("Password reset error: {Status}, Error: {Error}",
                    httpResponseMessage.StatusCode, error);
            }

            return httpResponseMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception sending password reset request");
            throw;
        }
    }

    public async Task<HttpResponseMessage> SendTwoFactorEnabledMessage(SetTwoFactorEnabledModel setTwoFactorEnabledModel)
    {
        _logger.LogInformation("Changing 2FA status to: {Enabled}", setTwoFactorEnabledModel.TwoFactorEnabled);

        try
        {
            string? token = await _localStorage.GetItemAsync<string>(ACCESS_KEY);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("No access token for 2FA status change");
                return null;
            }

            HttpRequestMessage httpRequest = new(HttpMethod.Post, "/api/auth/setTwoFactorEnabled");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _logger.LogDebug("Token for 2FA: {Token}", token.Substring(0, Math.Min(10, token.Length)) + "...");

            string jsonBody = JsonSerializer.Serialize(setTwoFactorEnabledModel);
            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");
            httpRequest.Content = content;

            _logger.LogDebug("Sending 2FA change request: {Body}", jsonBody);

            HttpResponseMessage httpResponseMessage = await _httpClientFactory.CreateClient("AuthorizedClient").SendAsync(httpRequest);
            _logger.LogDebug("2FA change response: {StatusCode}", httpResponseMessage.StatusCode);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("2FA status changed to: {Enabled}",
                    setTwoFactorEnabledModel.TwoFactorEnabled);
            }
            else
            {
                string error = await httpResponseMessage.Content.ReadAsStringAsync();
                _logger.LogWarning("2FA change error: {Status}, Error: {Error}",
                    httpResponseMessage.StatusCode, error);
            }

            return httpResponseMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception changing 2FA status");
            return null;
        }
    }

    public async Task<IEnumerable<AuthenticationScheme>> GetAuthenticationSchemesAsync()
    {
        _logger.LogInformation("Getting external authentication providers");

        try
        {
            IEnumerable<AuthenticationScheme>? schemes = await _httpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<AuthenticationScheme>>(
                "/api/auth/external-providers");

            int count = schemes?.Count() ?? 0;
            _logger.LogInformation("Got {Count} external providers", count);

            if (count > 0)
            {
                string names = string.Join(", ", schemes.Select(s => s.Name));
                _logger.LogDebug("Providers: {Names}", names);
            }

            return schemes ?? Enumerable.Empty<AuthenticationScheme>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting external providers");
            return Enumerable.Empty<AuthenticationScheme>();
        }
    }

    public async Task<ApplicationUser> GetCurrentUserAsync()
    {
        _logger.LogInformation("Getting current user");

        try
        {
            ApplicationUser? applicationUser = await _httpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<ApplicationUser>("/api/auth/current-user");

            if (applicationUser != null)
            {
                _logger.LogInformation("Current user: {Email}, ID: {Id}",
                    applicationUser.Email, applicationUser.Id);
            }
            else
            {
                _logger.LogWarning("Current user not found or not authorized");
            }

            return applicationUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return null;
        }
    }

    public async Task<HttpResponseMessage> SendChangePasswordMessageAsync(ChangePasswordModel changePasswordModel)
    {
        try
        {
            HttpResponseMessage changingPasswordHttpResponseMessage = await _httpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync(
                "/api/auth/set-password", changePasswordModel);

            _logger.LogDebug("Change password response: {StatusCode}",
                changingPasswordHttpResponseMessage.StatusCode);

            if (changingPasswordHttpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("Password changed successfully");
            }
            else
            {
                string error = await changingPasswordHttpResponseMessage.Content.ReadAsStringAsync();
                _logger.LogWarning("Change password error: {Status}, Error: {Error}",
                    changingPasswordHttpResponseMessage.StatusCode, error);
            }

            return changingPasswordHttpResponseMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception changing password");
            throw;
        }
    }

    public void AddDefaultRequestHeaderBearer(string accessToken)
    {
        var client = _httpClientFactory.CreateClient("AuthorizedClient");
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }

    public async Task<string?> GetCurrentAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedAccessToken))
            return _cachedAccessToken;

        try
        {
            _cachedAccessToken = await _localStorage.GetItemAsync<string>(ACCESS_KEY);
            return _cachedAccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting access token");
            return null;
        }
    }

    public async Task<LinkAccountIdResponse> StartVkIdLinkAsync                 ()
    {
        HttpResponseMessage response = await _httpClientFactory.CreateClient("AuthorizedClient").PostAsync("/api/auth/link-vkid", null);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Необходимо войти в систему");

        response.EnsureSuccessStatusCode();

        LinkAccountIdResponse? result = await response.Content.ReadFromJsonAsync<LinkAccountIdResponse>();

        if (result is null || string.IsNullOrWhiteSpace(result.Url))
            throw new InvalidOperationException("Сервер не вернул URL авторизации VK");

        return result;
    }

    public async Task<LinkAccountIdResponse> StartYandexIdLinkAsync()
    {
        HttpResponseMessage response = await _httpClientFactory.CreateClient("AuthorizedClient").PostAsync("/api/auth/link-yandexid", null);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Необходимо войти в систему");

        response.EnsureSuccessStatusCode();

        LinkAccountIdResponse? result = await response.Content.ReadFromJsonAsync<LinkAccountIdResponse>();

        if (result is null || string.IsNullOrWhiteSpace(result.Url))
            throw new InvalidOperationException("Сервер не вернул URL авторизации VK");

        return result;
    }
}