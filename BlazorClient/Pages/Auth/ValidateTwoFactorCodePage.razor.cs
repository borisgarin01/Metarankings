using BlazorClient.Auth;
using Blazored.Toast.Services;
using IdentityLibrary.Models;

namespace BlazorClient.Pages.Auth;

public partial class ValidateTwoFactorCodePage : ComponentBase
{
    private ValidateTwoFactorCodeModel model = new();

    [Parameter]
    public string UserId { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public IAuthService AuthService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private async Task HandleValidSubmitAsync()
    {
        try
        {
            AuthResponseDto tokenResponse = await AuthService.VerifyTwoFactorAsync(UserId, model.TwoFactorCode);

            if (tokenResponse is not null && !string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                await AuthService.StoreAccessTokenAsync(tokenResponse.AccessToken);
                NavigationManager.NavigateTo("/", forceLoad: true);
            }
            else
            {
                ToastService.ShowWarning("Неверный код подтверждения. Пожалуйста, попробуйте снова.");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Ошибка: {ex.Message}\t{ex.StackTrace}");
        }
    }
}
