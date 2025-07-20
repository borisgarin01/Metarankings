using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace BlazorClient.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;

    public JwtAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string savedToken = await _localStorageService.GetItemAsync<string>("tokenString");

        if (string.IsNullOrWhiteSpace(savedToken))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt")));
    }

    public void MarkUserAsAuthenticated(string email)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }, "apiauth"));
        var authenticationState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authenticationState);
    }

    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authenticationState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authenticationState);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        string payload = jwt.Split('.')[1];
        byte[] jsonBytes = ParseBase64WithoutPadding(payload);
        var claimsStringsDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        claimsStringsDictionary.TryGetValue(ClaimTypes.Role, out object roles);

        if (roles is not null)
        {
            if (IsItArray(roles.ToString()))
            {
                var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                foreach (string parsedRole in parsedRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                }
            }
            else
            {
                //Only one role
                claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
            }

            claimsStringsDictionary.Remove(ClaimTypes.Role);
        }

        claims.AddRange(claimsStringsDictionary.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

        return claims;
    }

    private static bool IsItArray(string value)
    {
        return value.ToString().Trim().StartsWith("[");
    }

    private byte[] ParseBase64WithoutPadding(string payload)
    {
        switch (payload.Length % 4)
        {
            case 2:
                payload += "==";
                break;
            case 3:
                payload += "=";
                break;
        }
        return Convert.FromBase64String(payload);
    }
}
