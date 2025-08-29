using Domain.Auth;
using IdentityLibrary.Models;
using System.Net;

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

    public async Task<string> LoginAsync(LoginModel loginModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest
                ^ response.StatusCode == HttpStatusCode.NotFound)
                throw new Exception(await response.Content.ReadAsStringAsync());
        }

        var token = await response.Content.ReadAsStringAsync();
        await _localStorage.SetItemAsync("authToken", token);

        ((JwtAuthenticationStateProvider)_authenticationStateProvider)
            .MarkUserAsAuthenticated(token); // Pass the token instead of email

        return token;
    }

    public async Task LogoutAsync()
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync("api/auth/logout", null);
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
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("api/auth/resetPasswordConfirm", resetPasswordConfirmModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> SendResetPasswordMessage(ResetPasswordModel resetPasswordModel)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("api/auth/resetPassword", resetPasswordModel);
        return httpResponseMessage;
    }
}