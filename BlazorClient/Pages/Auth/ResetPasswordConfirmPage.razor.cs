using BlazorClient.Auth;
using BlazorClient.PagesModels;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Auth;

public partial class ResetPasswordConfirmPage : ComponentBase
{
    [Inject]
    public IAuthService AuthService { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [SupplyParameterFromQuery]
    public string Email { get; set; }

    public ResetPasswordConfirmModel ResetPasswordConfirmModel { get; } = new();

    public async Task ResetPasswordConfirmAsync()
    {
        try
        {
            HttpResponseMessage resetPasswordHttpResponseMessage = await AuthService.SendResetPasswordConfirmMessage(new Domain.Auth.ResetPasswordConfirmModel(Email, ResetPasswordConfirmModel.NewPassword, ResetPasswordConfirmModel.ResetPasswordToken));

            if (resetPasswordHttpResponseMessage.IsSuccessStatusCode)
            {
                ToastService.ShowWarning("Password has been reset successully");
                NavigationManager.NavigateTo("/");
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
