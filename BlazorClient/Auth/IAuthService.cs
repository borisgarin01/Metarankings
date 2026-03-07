using Domain.Auth;
using IdentityLibrary.Models;
using System.Net;

namespace BlazorClient.Auth;

public interface IAuthService
{
    public Task RegisterAsync(RegisterModel registerModel);
    Task<LoginResponse> LoginAsync(LoginModel loginModel);
    public Task LogoutAsync();
    public Task<HttpResponseMessage> SendResetPasswordMessage(ResetPasswordModel resetPasswordModel);
    public Task<HttpResponseMessage> SendResetPasswordConfirmMessage(ResetPasswordConfirmModel resetPasswordModel);
    public Task<HttpResponseMessage> SendTwoFactorEnabledMessage(SetTwoFactorEnabledModel setTwoFactorEnabledModel);
    public Task<TokenResponse> VerifyTwoFactorAsync(string userId, string token);
    public Task StoreTokenAsync(string token);
    public Task<IEnumerable<AuthenticationScheme>> GetAuthenticationSchemesAsync();
}
