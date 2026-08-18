using Blazored.Toast.Services;
using Domain.Auth;
using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using System.Net;
using System.Text;

namespace BlazorClient.Auth;

public class AuthService : IAuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<AuthService> _logger;
    private readonly IToastService _toastService;
    private readonly IHttpClientFactory _httpClientFactory;

    private string? _cachedAccessToken;

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

        try
        {
            var refreshToken = await _localStorage.GetItemAsync<string>(REFRESH_KEY);

            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token missing");
                await LogoutAsync();
                return new AuthResponseDto(false, false, null, null, "Refresh token is missing");
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
            }

            _logger.LogWarning("Token refresh failed");
            await LogoutAsync();
            return new AuthResponseDto(false, false, null, null, "Failed to refresh token");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            await LogoutAsync();
            return new AuthResponseDto(false, false, null, null, $"Error refreshing token: {ex.Message}");
        }
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
            // Отправляем запрос на сервер для аннулирования токена
            var token = await GetCurrentAccessTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var client = _httpClientFactory.CreateClient("AuthorizedClient");
                    var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    await client.SendAsync(request);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error during logout request to server");
                }
            }

            // Очищаем локальное состояние
            await _localStorage.RemoveItemAsync(ACCESS_KEY);
            await _localStorage.RemoveItemAsync(REFRESH_KEY);
            _cachedAccessToken = null;

            var client2 = _httpClientFactory.CreateClient("AuthorizedClient");
            client2.DefaultRequestHeaders.Remove("Authorization");

            _logger.LogInformation("Logout completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during logout");
            // Пытаемся очистить состояние даже в случае ошибки
            try
            {
                await _localStorage.RemoveItemAsync(ACCESS_KEY);
                await _localStorage.RemoveItemAsync(REFRESH_KEY);
                _cachedAccessToken = null;
                _httpClientFactory.CreateClient("AuthorizedClient").DefaultRequestHeaders.Remove("Authorization");
            }
            catch { }
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
}