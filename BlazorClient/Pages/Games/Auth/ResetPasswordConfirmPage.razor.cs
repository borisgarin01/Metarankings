using BlazorClient.Auth;
using BlazorClient.PagesModels;

namespace BlazorClient.Pages.Games.Auth;

public partial class ResetPasswordConfirmPage : ComponentBase
{
    [Inject]
    public IAuthService AuthService { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

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
                await JSRuntime.InvokeVoidAsync("alert", "Password has been reset successully");
                NavigationManager.NavigateTo("/");
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", await resetPasswordHttpResponseMessage.Content.ReadAsStringAsync());
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"{ex.Message}\t{ex.StackTrace}");
        }
    }
}
