using BlazorClient.Auth;
using BlazorClient.PagesModels;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Games.Auth;

[Authorize]
public partial class SetTwoFactorEnabledPage : ComponentBase
{

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public IAuthService AuthService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public SetTwoFactorEnabledModel SetTwoFactorEnabledModel { get; } = new();

    public async Task SetTwoFactorEnabledAsync()
    {
        try
        {
            HttpResponseMessage resetPasswordHttpResponseMessage = await AuthService.SendTwoFactorEnabledMessage(new Domain.Auth.SetTwoFactorEnabledModel(SetTwoFactorEnabledModel.TwoFactorEnabled));

            if (resetPasswordHttpResponseMessage.IsSuccessStatusCode)
            {
                if (SetTwoFactorEnabledModel.TwoFactorEnabled)
                    await JSRuntime.InvokeVoidAsync("alert", "Теперь вы будете авторизоваться по 2Auth");
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Теперь вы не будете авторизоваться по 2Auth");
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
