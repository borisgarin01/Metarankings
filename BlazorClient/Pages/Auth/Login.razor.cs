using BlazorClient.Auth;
using Domain.Auth;
using IdentityLibrary.Models;

namespace BlazorClient.Pages.Auth;

public partial class Login : ComponentBase
{
    private IEnumerable<AuthenticationScheme> externalLogins = Enumerable.Empty<AuthenticationScheme>();

    [Inject] private IAuthService AuthService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }

    private LoginModel LoginModel { get; set; } = new();
    private string TwoFactorCode { get; set; } = string.Empty;
    private string UserIdFor2FA { get; set; } = string.Empty;
    private bool IsTwoFactorRequired { get; set; } = false;
    private bool IsLoading { get; set; } = false;
    private IEnumerable<AuthenticationScheme> ExternalLogins
    {
        get => externalLogins;
        set
        {
            externalLogins = value;
            StateHasChanged();
        }
    }
    protected override async Task OnInitializedAsync()
    {
        ExternalLogins = await AuthService.GetAuthenticationSchemesAsync();
    }

    public async Task LoginAsync()
    {
        if (IsTwoFactorRequired)
        {
            await VerifyTwoFactorAsync();
            return;
        }

        await PerformLoginAsync();
    }

    private async Task PerformLoginAsync()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();

            // First, try to login
            LoginResponse loginResponse = await AuthService.LoginAsync(LoginModel);

            if (loginResponse.RequiresTwoFactor)
            {
                // 2FA is required - show 2FA input
                IsTwoFactorRequired = true;
                UserIdFor2FA = loginResponse.UserId;
                await JSRuntime.InvokeVoidAsync("alert", "Код подтверждения отправлен на вашу почту");
            }
            else if (!string.IsNullOrWhiteSpace(loginResponse.Token))
            {
                // No 2FA required - store token and redirect
                await AuthService.StoreTokenAsync(loginResponse.Token);
                NavigationManager.NavigateTo("/", forceLoad: true);
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "Неверный логин или пароль");
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Ошибка: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task VerifyTwoFactorAsync()
    {
        if (string.IsNullOrWhiteSpace(TwoFactorCode))
        {
            await JSRuntime.InvokeVoidAsync("alert", "Введите код подтверждения");
            return;
        }

        try
        {
            IsLoading = true;
            StateHasChanged();

            // Verify 2FA code
            TokenResponse verifyResponse = await AuthService.VerifyTwoFactorAsync(UserIdFor2FA, TwoFactorCode);

            if (!string.IsNullOrWhiteSpace(verifyResponse.Token))
            {
                // 2FA successful - store token and redirect
                await AuthService.StoreTokenAsync(verifyResponse.Token);
                NavigationManager.NavigateTo("/", forceLoad: true);
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "Неверный код подтверждения");
                TwoFactorCode = string.Empty; // Clear the input
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Ошибка: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task ResendTwoFactorCode()
    {
        try
        {
            // Resend by calling login again
            LoginResponse loginResponse = await AuthService.LoginAsync(LoginModel);

            if (loginResponse.RequiresTwoFactor)
            {
                await JSRuntime.InvokeVoidAsync("alert", "Новый код подтверждения отправлен на вашу почту");
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "Ошибка при повторной отправке кода");
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Ошибка: {ex.Message}");
        }
    }

    private void CancelTwoFactor()
    {
        IsTwoFactorRequired = false;
        TwoFactorCode = string.Empty;
        UserIdFor2FA = string.Empty;
        StateHasChanged();
    }

    public Task SendResetPasswordMessageAsync()
    {
        NavigationManager.NavigateTo("/auth/resetPassword");
        return Task.CompletedTask;
    }

    public async Task DisplayErrors() => await JSRuntime.InvokeVoidAsync("alert", "DisplayErrors");

    public async Task<IEnumerable<AuthenticationScheme>> GetExternalLogins()
    {
        IEnumerable<AuthenticationScheme> authenticationSchemes = await AuthService.GetAuthenticationSchemesAsync();
        return authenticationSchemes;
    }

    public async Task ExternalLogin()
    {
        await JSRuntime.InvokeVoidAsync("alert", "External login");
    }
    public async Task LoginGoogle()
    {
        NavigationManager.NavigateTo($"/api/auth/login-google", forceLoad: true);
    }
}