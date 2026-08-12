using BlazorClient.Auth;
using BlazorClient.Modal;
using Blazored.Toast.Services;
using Domain.Auth;
using IdentityLibrary.Models;

namespace BlazorClient.Components.PagesComponents.Common;

public partial class LoginModal : ComponentBase
{
    private LoginModel LoginModel { get; set; } = new();
    private string TwoFactorCode { get; set; } = string.Empty;
    private string UserIdFor2FA { get; set; } = string.Empty;
    private bool IsTwoFactorRequired { get; set; } = false;
    private bool IsLoading { get; set; } = false;
    private IEnumerable<AuthenticationScheme> ExternalLogins { get; set; } = Enumerable.Empty<AuthenticationScheme>();

    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
    [Parameter] public EventCallback OnLoginSuccess { get; set; }

    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IToastService ToastService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            ExternalLogins = await AuthService.GetAuthenticationSchemesAsync();
        }
        catch
        {
            ExternalLogins = Enumerable.Empty<AuthenticationScheme>();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (IsVisible)
        {
            IsTwoFactorRequired = false;
            TwoFactorCode = string.Empty;
            UserIdFor2FA = string.Empty;
            LoginModel = new LoginModel();
            IsLoading = false;
        }
        await base.OnParametersSetAsync();
    }

    private async Task PerformLoginAsync()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();

            LoginResponseModel loginResponse = await AuthService.LoginAsync(LoginModel);

            if (loginResponse.RequiresTwoFactor)
            {
                IsTwoFactorRequired = true;
                UserIdFor2FA = loginResponse.UserId;
                ToastService.ShowInfo("Код подтверждения отправлен на вашу почту");
            }
            else if (!string.IsNullOrWhiteSpace(loginResponse.AccessToken))
            {
                await AuthService.StoreAccessTokenAsync(loginResponse.AccessToken);
                await AuthService.StoreRefreshTokenAsync(loginResponse.RefreshToken);
                AuthService.AddDefaultRequestHeaderBearer(loginResponse.AccessToken);

                ToastService.ShowSuccess("Вход выполнен успешно!");
                await OnLoginSuccess.InvokeAsync();
                await CloseAsync();
                NavigationManager.NavigateTo("/", forceLoad: true);
            }
            else
            {
                ToastService.ShowWarning("Неверный логин или пароль");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Ошибка: {ex.Message}");
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

            AuthResponseDto verifyResponse = await AuthService.VerifyTwoFactorAsync(UserIdFor2FA, TwoFactorCode);

            if (!string.IsNullOrWhiteSpace(verifyResponse.AccessToken) && !string.IsNullOrWhiteSpace(verifyResponse.RefreshToken))
            {
                await AuthService.StoreAccessTokenAsync(verifyResponse.AccessToken);
                await AuthService.StoreRefreshTokenAsync(verifyResponse.RefreshToken);

                ToastService.ShowSuccess("Вход выполнен успешно!");
                await OnLoginSuccess.InvokeAsync();
                await CloseAsync();
                NavigationManager.NavigateTo("/", forceLoad: true);
            }
            else
            {
                ToastService.ShowWarning("Неверный код подтверждения");
                TwoFactorCode = string.Empty;
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Ошибка: {ex.Message}");
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
            LoginResponseModel loginResponse = await AuthService.LoginAsync(LoginModel);

            if (loginResponse.RequiresTwoFactor)
            {
                ToastService.ShowInfo("Новый код подтверждения отправлен на вашу почту");
            }
            else
            {
                ToastService.ShowError("Ошибка при повторной отправке кода");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Ошибка: {ex.Message}");
        }
    }

    private void CancelTwoFactor()
    {
        IsTwoFactorRequired = false;
        TwoFactorCode = string.Empty;
        UserIdFor2FA = string.Empty;
        StateHasChanged();
    }

    private async Task LoginExternal(AuthenticationScheme login)
    {
        string provider = login.DisplayName?.ToLower() switch
        {
            "google" => "google",
            "vkontakte" => "vkontakte",
            "github" => "github",
            "mailru" => "mailru",
            "vk id" => "vkid",
            "yandex" => "yandex",
            _ => login.DisplayName?.ToLower() ?? ""
        };

        if (!string.IsNullOrWhiteSpace(provider))
        {
            await CloseAsync();
            NavigationManager.NavigateTo($"/api/auth/login-{provider}", forceLoad: true);
        }
    }

    private async Task CloseAsync()
    {
        IsVisible = false;
        await IsVisibleChanged.InvokeAsync(false);
    }

    private async Task CloseOnOverlayAsync(MouseEventArgs e)
    {
        await CloseAsync();
    }
}