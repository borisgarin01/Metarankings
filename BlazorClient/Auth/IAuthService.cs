using Domain.Auth;
using IdentityLibrary.DTOs;
using IdentityLibrary.Models;

namespace BlazorClient.Auth;

public interface IAuthService
{
    // Регистрация и вход
    Task RegisterAsync(RegisterModel registerModel);
    Task<LoginResponseModel> LoginAsync(LoginModel loginModel);
    Task LogoutAsync();

    // Управление токенами
    Task<AuthResponseDto> RefreshTokenAsync();
    Task StoreAccessTokenAsync(string token);
    Task StoreRefreshTokenAsync(string token);
    Task<string?> GetCurrentAccessTokenAsync(); // НОВЫЙ МЕТОД
    void AddDefaultRequestHeaderBearer(string accessToken);

    // 2FA
    Task<AuthResponseDto> VerifyTwoFactorAsync(string userId, string token);

    // Управление пользователем
    Task<ApplicationUser> GetCurrentUserAsync();
    Task<IEnumerable<AuthenticationScheme>> GetAuthenticationSchemesAsync();

    // Работа с паролем
    Task<HttpResponseMessage> SendResetPasswordMessage(ResetPasswordModel resetPasswordModel);
    Task<HttpResponseMessage> SendResetPasswordConfirmMessage(ResetPasswordConfirmModel resetPasswordModel);
    Task<HttpResponseMessage> SendChangePasswordMessageAsync(ChangePasswordModel changePasswordModel);
    Task<HttpResponseMessage> SendTwoFactorEnabledMessage(SetTwoFactorEnabledModel setTwoFactorEnabledModel);
}