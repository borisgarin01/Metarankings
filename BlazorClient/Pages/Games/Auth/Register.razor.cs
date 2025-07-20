using BlazorClient.Auth;
using IdentityLibrary.Models;

namespace BlazorClient.Pages.Games.Auth;

public partial class Register : ComponentBase
{
    private bool showErrors = false;
    private IEnumerable<string> errors;

    [Inject]
    public IAuthService AuthService { get; set; }

    public RegisterModel RegisterModel { get; } = new RegisterModel();

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public async Task RegisterAsync()
    {
        showErrors = false;

        try
        {
            string jsonToken = await AuthService.RegisterAsync(RegisterModel);
            NavigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            showErrors = true;
        }
    }
}
