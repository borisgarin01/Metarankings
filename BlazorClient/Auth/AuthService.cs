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
        await _localStorage.RemoveItemAsync("authToken");
        ((JwtAuthenticationStateProvider)_authenticationStateProvider)
            .MarkUserAsLoggedOut();
    }

    public async Task<string> RegisterAsync(RegisterModel registerModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerModel);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var token = await response.Content.ReadAsStringAsync();
        await _localStorage.SetItemAsync("authToken", token);

        ((JwtAuthenticationStateProvider)_authenticationStateProvider)
            .MarkUserAsAuthenticated(registerModel.UserEmail);

        return token;
    }
}