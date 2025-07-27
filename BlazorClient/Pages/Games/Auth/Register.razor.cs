using BlazorClient.Auth;
using IdentityLibrary.Models;

namespace BlazorClient.Pages.Games.Auth;

public partial class Register : ComponentBase
{

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public IAuthService AuthService { get; set; }

    public RegisterModel RegisterModel { get; } = new RegisterModel();

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public async Task RegisterAsync()
    {
        try
        {
            string jsonToken = await AuthService.RegisterAsync(RegisterModel);
            if (!string.IsNullOrWhiteSpace(jsonToken))
                NavigationManager.NavigateTo("/");
            else
                await JSRuntime.InvokeVoidAsync("alert", "Непредвиденная ошибка при регистрации");
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", ex.Message);
        }
    }
}
