using BlazorClient.Auth;
using Blazored.Toast.Services;
using Domain.Auth;
using IdentityLibrary.Models;

namespace BlazorClient.Pages.Auth;

public partial class Login : ComponentBase
{
    private IEnumerable<AuthenticationScheme> externalLogins = Enumerable.Empty<AuthenticationScheme>();

    [Inject]
    private IAuthService AuthService { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Inject]
    private IToastService ToastService { get; set; }

    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

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
            LoginResponseModel loginResponse = await AuthService.LoginAsync(LoginModel);

            if (loginResponse.RequiresTwoFactor)
            {
                // 2FA is required - show 2FA input
                IsTwoFactorRequired = true;
                UserIdFor2FA = loginResponse.UserId;
                ToastService.ShowInfo("Код подтверждения отправлен на вашу почту");
            }
            else if (!string.IsNullOrWhiteSpace(loginResponse.AccessToken) && !string.IsNullOrWhiteSpace(loginResponse.RefreshToken))
            {
                await ((JwtAuthenticationStateProvider)AuthenticationStateProvider).MarkUserAsAuthenticated(loginResponse);

                NavigationManager.NavigateTo("/", forceLoad: true);
            }
            else
                ToastService.ShowError("Неверный логин или пароль");
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"{ex.Message}\t{ex.StackTrace}");
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
            ToastService.ShowWarning("Введите код подтверждения");
            return;
        }

        try
        {
            IsLoading = true;
            StateHasChanged();

            TokenResponse verifyResponse = await AuthService.VerifyTwoFactorAsync(UserIdFor2FA, TwoFactorCode);

            if (!string.IsNullOrWhiteSpace(verifyResponse.AccessToken))
            {
                await AuthService.StoreAccessTokenAsync(verifyResponse.AccessToken);
                await AuthService.StoreRefreshTokenAsync(verifyResponse.RefreshToken);

                // Создаем LoginResponseModel для сохранения в sessionState
                LoginResponseModel loginResponse = new(UserIdFor2FA, verifyResponse.AccessToken, verifyResponse.TokenExpired, verifyResponse.RefreshToken, false);

                await ((JwtAuthenticationStateProvider)AuthenticationStateProvider)
                    .MarkUserAsAuthenticated(loginResponse);

                NavigationManager.NavigateTo("/", forceLoad: true);
            }
            else
            {
                ToastService.ShowError("Неверный код подтверждения");
                TwoFactorCode = string.Empty;
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"{ex.Message}\t{ex.StackTrace}");
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
            LoginResponseModel loginResponse = await AuthService.LoginAsync(LoginModel);

            if (loginResponse.RequiresTwoFactor)
                ToastService.ShowInfo("Новый код подтверждения отправлен на вашу почту");
            else
                ToastService.ShowError("Ошибка при повторной отправке кода");
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"{ex.Message}\t{ex.StackTrace}");
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

    public async Task<IEnumerable<AuthenticationScheme>> GetExternalLogins()
    {
        IEnumerable<AuthenticationScheme> authenticationSchemes = await AuthService.GetAuthenticationSchemesAsync();
        return authenticationSchemes;
    }

    public async Task LoginGoogle()
    {
        NavigationManager.NavigateTo($"/api/auth/login-google", forceLoad: true);
    }

    public async Task LoginVkontakte()
    {
        NavigationManager.NavigateTo($"/api/auth/login-vkontakte", forceLoad: true);
    }

    public async Task LoginGithub()
    {
        NavigationManager.NavigateTo($"/api/auth/login-github", forceLoad: true);
    }

    public async Task LoginMailRu()
    {
        NavigationManager.NavigateTo($"/api/auth/login-mailru", forceLoad: true);
    }

    public async Task LoginVKID()
    {
        NavigationManager.NavigateTo($"/api/auth/login-vkid", forceLoad: true);
    }

    public async Task LoginYandex()
    {
        NavigationManager.NavigateTo($"/api/auth/login-yandex", forceLoad: true);
    }
}