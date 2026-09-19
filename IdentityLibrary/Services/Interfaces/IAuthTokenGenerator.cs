using IdentityLibrary.DTOs;

namespace API.Auth;

public interface IAuthTokenGenerator
{
    Task<string> GenerateAccessToken(ApplicationUser user);
    string GenerateRefreshToken();
    string GenerateCodeVerifier();
    string GenerateCodeChallenge(string verifier);
    Task<string?> ExchangeVkCodeForUserIdAsync(string code, string codeVerifier, string scheme, string host, string deviceId);
}