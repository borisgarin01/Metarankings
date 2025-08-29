using BlazorClient.Auth;
using BlazorClient.PagesModels;

namespace BlazorClient.Pages.Games.Auth;

public partial class ResetPasswordPage : ComponentBase
{
    [Inject]
    public IJSRuntime JSRuntime { get; set; }

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
                await JSRuntime.InvokeVoidAsync("alert", await resetPasswordHttpResponseMessage.Content.ReadAsStringAsync());
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"{ex.Message}\t{ex.StackTrace}");
        }
    }
}
