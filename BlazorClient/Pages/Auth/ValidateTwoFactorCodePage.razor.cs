using BlazorClient.Auth;
using IdentityLibrary.Models;

namespace BlazorClient.Pages.Auth;

public partial class ValidateTwoFactorCodePage : ComponentBase
{
    [Parameter]
    public string UserId { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public IAuthService AuthService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private ValidateTwoFactorCodeModel model = new();

    private async Task HandleValidSubmitAsync()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("console.log", $"Validating 2FA code: {model.TwoFactorCode} for UserId: {UserId}");

            TokenResponse tokenResponse = await AuthService.VerifyTwoFactorAsync(UserId, model.TwoFactorCode);

            if (tokenResponse is not null && !string.IsNullOrWhiteSpace(tokenResponse.Token) && string.IsNullOrWhiteSpace(tokenResponse.Error))
            {
                await AuthService.StoreTokenAsync(tokenResponse.Token);
                NavigationManager.NavigateTo("/", forceLoad: true);
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "Неверный код подтверждения. Пожалуйста, попробуйте снова.");
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Ошибка: {ex.Message}");
        }
    }
}
