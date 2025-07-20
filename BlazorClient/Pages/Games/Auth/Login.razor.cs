using BlazorClient.Auth;
using IdentityLibrary.Models;

namespace BlazorClient.Pages.Games.Auth;

public partial class Login : ComponentBase
{
    private bool showErrors = false;
    private IEnumerable<string> errors;

    [Inject]
    public IAuthService AuthService { get; set; }

    public LoginModel LoginModel { get; } = new LoginModel();

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public async Task LoginAsync()
    {
        showErrors = false;

        try
        {
            string jsonToken = await AuthService.LoginAsync(LoginModel);
            if (!string.IsNullOrWhiteSpace(jsonToken))
                NavigationManager.NavigateTo("/");

        }
        catch (Exception ex)
        {
            showErrors = true;
        }
    }
}
