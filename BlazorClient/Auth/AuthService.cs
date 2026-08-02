using API.Controllers.Auth;
using Blazored.Toast.Services;
using Domain.Auth;
using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using System.Net;
using System.Text;

namespace BlazorClient.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<AuthService> _logger;
    private readonly IToastService _toastService;

    public AuthService(HttpClient httpClient,
                      ILocalStorageService localStorage,
                      ILogger<AuthService> logger,
                      IToastService toastService)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _logger = logger;
        _toastService = toastService;
    }

    public async Task<LoginResponseModel> LoginAsync(LoginModel loginModel)
    {
        _logger.LogInformation("Login attempt for user: {Email}", loginModel.UserEmail);

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponseModel>();
                _logger.LogInformation("Login successful for {Email}, TwoFactor: {TwoFactor}",
                    loginModel.UserEmail, result?.RequiresTwoFactor);
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
            string? refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");
            _logger.LogDebug("Refresh token from localStorage: {TokenStatus}",
                string.IsNullOrEmpty(refreshToken) ? "MISSING" : "present");

            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token missing");
                return new AuthResponseDto(false, false, null, null, "Refresh token is missing");
            }

            RefreshTokenRequest refreshRequest = new()
            {
                RefreshToken = refreshToken
            };

            _logger.LogInformation("Sending refresh token request");
            _logger.LogDebug("RefreshToken: {Token}", refreshToken);

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/auth/refresh-token", refreshRequest);
            _logger.LogDebug("Response status: {StatusCode}", response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Response content: {Content}", responseContent);

                AuthResponseDto? tokenResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent);

                if (tokenResponse != null && tokenResponse.IsAuthSuccessful)
                {
                    _logger.LogInformation("Token refreshed successfully");

                    await StoreAccessTokenAsync(tokenResponse.AccessToken);
                    await StoreRefreshTokenAsync(tokenResponse.RefreshToken);

                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

                    _logger.LogInformation("New tokens saved - AccessToken length: {AccessLength}, RefreshToken length: {RefreshLength}",
                        tokenResponse.AccessToken?.Length ?? 0, tokenResponse.RefreshToken?.Length ?? 0);

                    return tokenResponse;
                }
                else
                {
                    _logger.LogWarning("Token refresh returned Success=false");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Token refresh failed. Status: {Status}, Error: {Error}",
                    response.StatusCode, errorContent);
            }

            _logger.LogWarning("Logging out due to failed token refresh");
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

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/auth/ConfirmLoginViaEmail", request);
            _logger.LogDebug("2FA response: StatusCode = {StatusCode}", response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Verification response: {Content}", responseContent);

                var result = JsonSerializer.Deserialize<AuthResponseDto>(responseContent);

                if (result != null && result.IsAuthSuccessful)
                {
                    _logger.LogInformation("2FA verification successful for {UserId}", userId);

                    await StoreAccessTokenAsync(result.AccessToken);
                    await StoreRefreshTokenAsync(result.RefreshToken);

                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);

                    return result;
                }
                else
                {
                    _logger.LogWarning("2FA verification returned Success=false for {UserId}", userId);
                }
            }

            var error = await response.Content.ReadAsStringAsync();
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
        _logger.LogInformation("Saving access token, length: {Length}", token?.Length ?? 0);

        try
        {
            await _localStorage.SetItemAsync<string>("accessToken", token);
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            _logger.LogDebug("Access token saved and set in HttpClient");

            var saved = await _localStorage.GetItemAsync<string>("accessToken");
            _logger.LogDebug("Save verification: {Saved}", string.IsNullOrEmpty(saved) ? "FAILED" : "success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving access token");
            throw;
        }
    }

    public async Task StoreRefreshTokenAsync(string refreshToken)
    {
        _logger.LogInformation("Saving refresh token, length: {Length}", refreshToken?.Length ?? 0);

        try
        {
            await _localStorage.SetItemAsync<string>("refreshToken", refreshToken);
            _logger.LogDebug("Refresh token saved");

            var saved = await _localStorage.GetItemAsync<string>("refreshToken");
            _logger.LogDebug("Refresh save verification: {Saved}", string.IsNullOrEmpty(saved) ? "FAILED" : "success");
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
            string? token = await _localStorage.GetItemAsync<string>("accessToken");

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Access token missing during logout");
                await _localStorage.RemoveItemAsync("accessToken");
                await _localStorage.RemoveItemAsync("refreshToken");
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return;
            }

            HttpRequestMessage httpRequest = new(HttpMethod.Post, "/api/auth/logout");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _logger.LogDebug("Sending logout request");

            HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequest);
            _logger.LogDebug("Logout response: {StatusCode}", httpResponseMessage.StatusCode);

            await _localStorage.RemoveItemAsync("accessToken");
            await _localStorage.RemoveItemAsync("refreshToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("Logout successful, tokens removed");
            }
            else
            {
                var error = await httpResponseMessage.Content.ReadAsStringAsync();
                _logger.LogWarning("Logout returned error: {Status}, Error: {Error}",
                    httpResponseMessage.StatusCode, error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during logout");
            try
            {
                await _localStorage.RemoveItemAsync("accessToken");
                await _localStorage.RemoveItemAsync("refreshToken");
                _httpClient.DefaultRequestHeaders.Authorization = null;
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
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/auth/register", registerModel);
            _logger.LogDebug("Registration response: StatusCode = {StatusCode}", httpResponseMessage.StatusCode);

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest ||
                httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                var error = await httpResponseMessage.Content.ReadAsStringAsync();
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
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync(
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
                var error = await httpResponseMessage.Content.ReadAsStringAsync();
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
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync<ResetPasswordModel>(
                "/api/auth/resetPassword", resetPasswordModel);

            _logger.LogDebug("Password reset response: {StatusCode}", httpResponseMessage.StatusCode);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("Password reset request sent for {Email}",
                    resetPasswordModel.Email);
            }
            else
            {
                var error = await httpResponseMessage.Content.ReadAsStringAsync();
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
            string? token = await _localStorage.GetItemAsync<string>("accessToken");

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

            HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequest);
            _logger.LogDebug("2FA change response: {StatusCode}", httpResponseMessage.StatusCode);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("2FA status changed to: {Enabled}",
                    setTwoFactorEnabledModel.TwoFactorEnabled);
            }
            else
            {
                var error = await httpResponseMessage.Content.ReadAsStringAsync();
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
            IEnumerable<AuthenticationScheme>? schemes = await _httpClient.GetFromJsonAsync<IEnumerable<AuthenticationScheme>>(
                "/api/auth/external-providers");

            int count = schemes?.Count() ?? 0;
            _logger.LogInformation("Got {Count} external providers", count);

            if (count > 0)
            {
                var names = string.Join(", ", schemes.Select(s => s.Name));
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
            ApplicationUser? applicationUser = await _httpClient.GetFromJsonAsync<ApplicationUser>("/api/auth/current-user");

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
            HttpResponseMessage changingPasswordHttpResponseMessage = await _httpClient.PostAsJsonAsync(
                "/api/auth/set-password", changePasswordModel);

            _logger.LogDebug("Change password response: {StatusCode}",
                changingPasswordHttpResponseMessage.StatusCode);

            if (changingPasswordHttpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("Password changed successfully");
            }
            else
            {
                var error = await changingPasswordHttpResponseMessage.Content.ReadAsStringAsync();
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
}