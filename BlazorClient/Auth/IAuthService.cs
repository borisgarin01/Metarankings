using Domain.Auth;
using IdentityLibrary.Models;

namespace BlazorClient.Auth;

public interface IAuthService
{
    public Task RegisterAsync(RegisterModel registerModel);
    public Task<string> LoginAsync(LoginModel loginModel);
    public Task LogoutAsync();
    public Task<HttpResponseMessage> SendResetPasswordMessage(ResetPasswordModel resetPasswordModel);
    public Task<HttpResponseMessage> SendResetPasswordConfirmMessage(ResetPasswordConfirmModel resetPasswordModel);
}
