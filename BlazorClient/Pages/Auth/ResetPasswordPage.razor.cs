using BlazorClient.Auth;
using BlazorClient.PagesModels;
using Blazored.Toast.Services;

namespace BlazorClient.Pages.Auth;

public partial class ResetPasswordPage : ComponentBase
{
    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public IAuthService AuthService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public ResetPasswordModel ResetPasswordModel { get; } = new();

    public async Task ResetPasswordAsync()
    {
        try
        {
            HttpResponseMessage resetPasswordHttpResponseMessage = await AuthService.SendResetPasswordMessage(new Domain.Auth.ResetPasswordModel(ResetPasswordModel.Email));

            if (resetPasswordHttpResponseMessage.IsSuccessStatusCode)
            {
                NavigationManager.NavigateTo($"/auth/ResetPasswordConfirm?email={ResetPasswordModel.Email}");
            }
            else
            {
                ToastService.ShowError(await resetPasswordHttpResponseMessage.Content.ReadAsStringAsync());
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"{ex.Message}\t{ex.StackTrace}");
        }
    }
}
