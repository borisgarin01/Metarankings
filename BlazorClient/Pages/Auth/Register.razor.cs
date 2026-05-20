using BlazorClient.Auth;
using Blazored.Toast.Services;
using IdentityLibrary.Models;

namespace BlazorClient.Pages.Auth;

public partial class Register : ComponentBase
{

    [Inject]
    public IToastService ToastService { get; set; }

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
            ToastService.ShowInfo("На Ваш адрес электронной почты отправлено письмо для перехода по ссылке для подтверждения аккаунта");
            NavigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"{ex.Message}\t{ex.StackTrace}");
        }
    }
}
