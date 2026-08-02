using IdentityLibrary.DTOs;

namespace API.Auth
{
    public interface IAuthTokenGenerator
    {
        Task<string> GenerateAccessToken(ApplicationUser user);
        string GenerateRefreshToken();
    }
}