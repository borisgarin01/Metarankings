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
                // Verify token is saved
                var savedToken = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

                NavigationManager.NavigateTo("/", forceLoad: true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            await JSRuntime.InvokeVoidAsync("alert", $"Login failed: {ex.Message}");
        }
    }

    public async Task DisplayErrors()
    {
        await JSRuntime.InvokeVoidAsync("alert", "DisplayErrors");
    }

    public Task SendResetPasswordMessageAsync()
    {
        NavigationManager.NavigateTo("/auth/resetPassword");
        return Task.CompletedTask;
    }
}
