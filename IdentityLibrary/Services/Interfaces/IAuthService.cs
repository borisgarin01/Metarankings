using IdentityLibrary.Models;

namespace IdentityLibrary.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginModel loginModel);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request);
    Task LogoutAsync(string refreshToken);
    Task LogoutAllAsync(long userId);
}
