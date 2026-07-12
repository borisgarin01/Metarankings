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

    public AuthService(HttpClient httpClient,
                      ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    [Inject]
    public IToastService ToastService { get; set; }

    public async Task<LoginResponseModel> LoginAsync(LoginModel loginModel)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponseModel>();
        }

        throw new Exception(await response.Content.ReadAsStringAsync());
    }

    public async Task<TokenResponse> RefreshTokenAsync()
    {
        // Получаем refresh token из localStorage
        string? refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

        if (string.IsNullOrEmpty(refreshToken))
        {
            return new TokenResponse(false, string.Empty, 0, "Refresh token is missing");
        }

        // Создаем запрос на обновление токена
        RefreshTokenRequest refreshRequest = new()
        {
            RefreshToken = refreshToken
        };

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/refresh-token", refreshRequest);

            if (response.IsSuccessStatusCode)
            {
                TokenResponse? tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

                if (tokenResponse != null && tokenResponse.Success)
                {
                    // Сохраняем новые токены
                    await StoreAccessTokenAsync(tokenResponse.AccessToken);
                    await StoreRefreshTokenAsync(tokenResponse.RefreshToken);

                    // Устанавливаем новый access token в HttpClient
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

                    return tokenResponse;
                }
            }

            // Если обновление не удалось - разлогиниваем пользователя
            await LogoutAsync();
            return new TokenResponse(false, string.Empty, 0, "Failed to refresh token");
        }
        catch (Exception ex)
        {
            await LogoutAsync();
            return new TokenResponse(false, string.Empty, 0, $"Error refreshing token: {ex.Message}");
        }
    }

    public async Task<TokenResponse> VerifyTwoFactorAsync(string userId, string token)
    {
        ConfirmLoginModel request = new(userId, token);
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/ConfirmLoginViaEmail", request);

        if (response.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<TokenResponse>(await response.Content.ReadAsStreamAsync());
        }

        return new TokenResponse(false, string.Empty, 0, "Ошибка верификации");
    }

    public async Task StoreAccessTokenAsync(string token)
    {
        // Store token in localStorage
        await _localStorage.SetItemAsync<string>("accessToken", token);

        // Also set in HTTP client headers
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task StoreRefreshTokenAsync(string refreshToken)
    {
        await _localStorage.SetItemAsync<string>("refreshToken", refreshToken);
    }

    public async Task LogoutAsync()
    {
        string? token = await _localStorage.GetItemAsync<string>("accessToken");

        if (token is null)
            return;

        HttpRequestMessage httpRequest = new(HttpMethod.Post, "api/auth/logout");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequest);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            await _localStorage.RemoveItemAsync("accessToken");
            await _localStorage.RemoveItemAsync("refreshToken");
        }
    }

    public async Task RegisterAsync(RegisterModel registerModel)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("api/auth/register", registerModel);

        if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest ^ httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());
    }

    public async Task<HttpResponseMessage> SendResetPasswordConfirmMessage(ResetPasswordConfirmModel resetPasswordConfirmModel)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("api/auth/resetPasswordConfirm", resetPasswordConfirmModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> SendResetPasswordMessage(ResetPasswordModel resetPasswordModel)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync<ResetPasswordModel>("api/auth/resetPassword", resetPasswordModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> SendTwoFactorEnabledMessage(SetTwoFactorEnabledModel setTwoFactorEnabledModel)
    {
        string? token = await _localStorage.GetItemAsync<string>("accessToken");

        if (token is not null)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, "api/auth/setTwoFactorEnabled");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string jsonBody = System.Text.Json.JsonSerializer.Serialize(setTwoFactorEnabledModel);

            // 2. Create StringContent with the JSON string and set Content-Type header
            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");

            httpRequest.Content = content;

            HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequest);

            return httpResponseMessage;
        }

        return null;
    }

    public async Task<IEnumerable<AuthenticationScheme>> GetAuthenticationSchemesAsync()
    {
        IEnumerable<AuthenticationScheme>? schemes = await _httpClient.GetFromJsonAsync<IEnumerable<AuthenticationScheme>>("api/auth/external-providers");
        return schemes;
    }

    public Task<ApplicationUser> GetCurrentUserAsync()
    {
        Task<ApplicationUser?> applicaitonUser = _httpClient.GetFromJsonAsync<ApplicationUser>("api/auth/current-user");

        return applicaitonUser;
    }

    public async Task<HttpResponseMessage> SendChangePasswordMessageAsync(ChangePasswordModel changePasswordModel)
    {
        HttpResponseMessage changingPasswordHttpResponseMessage = await _httpClient.PostAsJsonAsync("api/auth/set-password", changePasswordModel);

        return changingPasswordHttpResponseMessage;
    }
}