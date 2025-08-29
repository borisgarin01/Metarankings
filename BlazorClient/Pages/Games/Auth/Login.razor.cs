using BlazorClient.Auth;
using IdentityLibrary.Models;

namespace BlazorClient.Pages.Games.Auth;

public partial class Login : ComponentBase
{
    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public IAuthService AuthService { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    public LoginModel LoginModel { get; } = new LoginModel();

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public async Task LoginAsync()
    {
        try
        {
            string token = await AuthService.LoginAsync(LoginModel);
            if (!string.IsNullOrWhiteSpace(token))
            {
                // No need to call GetAuthenticationStateAsync here
                NavigationManager.NavigateTo("/", forceLoad: true); // forceLoad ensures full state refresh
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "Неверный логин или пароль");
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", ex.Message);
        }
    }

    public Task SendResetPasswordMessageAsync()
    {
        NavigationManager.NavigateTo("/auth/resetPassword");
        return Task.CompletedTask;
    }
}
