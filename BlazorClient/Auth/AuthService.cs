using Blazored.LocalStorage;
using IdentityLibrary.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace BlazorClient.Auth;

public sealed class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorageService;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorageService = localStorageService;
    }

    public async Task<string> LoginAsync(LoginModel loginModel)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync<LoginModel>($"/api/Auth/Login", loginModel);
        return await TrySaveAuthToken(loginModel.UserEmail, httpResponseMessage);
    }

    public async Task<string> RegisterAsync(RegisterModel registerModel)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync<RegisterModel>($"/api/Auth/Register", registerModel);
        return await TrySaveAuthToken(registerModel.UserEmail, httpResponseMessage);
    }

    private async Task<string> TrySaveAuthToken(string email, HttpResponseMessage httpResponseMessage)
    {
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            string authJwt = await httpResponseMessage.Content.ReadAsStringAsync();
            await _localStorageService.SetItemAsync<string>("token", authJwt);
            ((JwtAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(email);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authJwt);
            return authJwt;
        }
        return string.Empty;
    }

    public async Task Logout()
    {
        await _localStorageService.RemoveItemAsync("token");
        ((JwtAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
