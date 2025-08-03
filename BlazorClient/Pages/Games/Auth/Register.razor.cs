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
            await AuthService.RegisterAsync(RegisterModel);
            await JSRuntime.InvokeVoidAsync("alert", "На Ваш адрес электронной почты отправлено письмо для перехода по ссылке для подтверждения аккаунта");
            NavigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", ex.Message);
        }
    }
}
