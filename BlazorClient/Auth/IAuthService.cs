using IdentityLibrary.Models;

namespace BlazorClient.Auth;

public interface IAuthService
{
    public Task RegisterAsync(RegisterModel registerModel);
    public Task<string> LoginAsync(LoginModel loginModel);
    public Task LogoutAsync();
}
