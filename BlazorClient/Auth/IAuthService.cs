using Domain.Auth;
using IdentityLibrary.DTOs;
using IdentityLibrary.Models;

namespace BlazorClient.Auth;

public interface IAuthService
{
    Task RegisterAsync(RegisterModel registerModel);
    Task<LoginResponseModel> LoginAsync(LoginModel loginModel);
    Task LogoutAsync();
    Task<HttpResponseMessage> SendResetPasswordMessage(ResetPasswordModel resetPasswordModel);
    Task<HttpResponseMessage> SendResetPasswordConfirmMessage(ResetPasswordConfirmModel resetPasswordModel);
    Task<HttpResponseMessage> SendTwoFactorEnabledMessage(SetTwoFactorEnabledModel setTwoFactorEnabledModel);
    Task<AuthResponseDto> VerifyTwoFactorAsync(string userId, string token);
    Task StoreAccessTokenAsync(string token);
    Task StoreRefreshTokenAsync(string token);
    Task<IEnumerable<AuthenticationScheme>> GetAuthenticationSchemesAsync();
    Task<ApplicationUser> GetCurrentUserAsync();
    Task<HttpResponseMessage> SendChangePasswordMessageAsync(ChangePasswordModel changePasswordModel);
    Task<AuthResponseDto> RefreshTokenAsync();
    void AddDefaultRequestHeaderBearer(string accessToken);
}
