using Domain.Auth;
using IdentityLibrary.Models;
using System.Net;
using System.Text;

namespace BlazorClient.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthService(HttpClient httpClient,
                      ILocalStorageService localStorage,
                      AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authenticationStateProvider = authenticationStateProvider;
    }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public async Task<LoginResponse> LoginAsync(LoginModel loginModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        return new LoginResponse();
    }

    public async Task<TokenResponse> VerifyTwoFactorAsync(string userId, string token)
    {
        var request = new { UserId = userId, TwoFactorToken = token };
        var response = await _httpClient.PostAsJsonAsync("api/auth/ConfirmLoginViaEmail", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TokenResponse>();
        }

        return new TokenResponse { Error = "Ошибка верификации" };
    }

    public async Task StoreTokenAsync(string token)
    {
        // Store token in localStorage
        await _localStorage.SetItemAsync<string>("authToken", token);

        // Also set in HTTP client headers
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task LogoutAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (token is null)
            return;

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequest);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((JwtAuthenticationStateProvider)_authenticationStateProvider)
                .MarkUserAsLoggedOut();
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
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (token is not null)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/resetPasswordConfirm");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequest);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                await _localStorage.RemoveItemAsync("authToken");
                ((JwtAuthenticationStateProvider)_authenticationStateProvider)
                    .MarkUserAsLoggedOut();
                return httpResponseMessage;
            }
        }
        return null;
    }

    public async Task<HttpResponseMessage> SendResetPasswordMessage(ResetPasswordModel resetPasswordModel)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (token is not null)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/resetPassword");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequest);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                await _localStorage.RemoveItemAsync("authToken");
                ((JwtAuthenticationStateProvider)_authenticationStateProvider)
                    .MarkUserAsLoggedOut();
                return httpResponseMessage;
            }
        }
        return null;
    }

    public async Task<HttpResponseMessage> SendTwoFactorEnabledMessage(SetTwoFactorEnabledModel setTwoFactorEnabledModel)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (token is not null)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/setTwoFactorEnabled");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string jsonBody = System.Text.Json.JsonSerializer.Serialize(setTwoFactorEnabledModel);

            // 2. Create StringContent with the JSON string and set Content-Type header
            HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            httpRequest.Content = content;

            HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequest);

            return httpResponseMessage;
        }

        return null;
    }
}