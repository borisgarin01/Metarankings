using BlazorClient.Auth;
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
    private string LoginError { get; set; } = string.Empty;
    private string TwoFactorError { get; set; } = string.Empty;
    private IEnumerable<AuthenticationScheme> ExternalLogins { get; set; } = Enumerable.Empty<AuthenticationScheme>();

    [Parameter] 
    public bool IsVisible { get; set; }
    
    [Parameter] 
    public EventCallback<bool> IsVisibleChanged { get; set; }
    
    [Parameter] 
    public EventCallback OnLoginSuccess { get; set; }

    [Inject] 
    private IAuthService AuthService { get; set; } = default!;
    
    [Inject] 
    private NavigationManager NavigationManager { get; set; } = default!;
    
    [Inject] 
    private IToastService ToastService { get; set; } = default!;

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
            // Сброс всех полей при открытии
            IsTwoFactorRequired = false;
            TwoFactorCode = string.Empty;
            UserIdFor2FA = string.Empty;
            LoginModel = new LoginModel();
            IsLoading = false;
            LoginError = string.Empty;
            TwoFactorError = string.Empty;
        }
        await base.OnParametersSetAsync();
    }

    private async Task PerformLoginAsync()
    {
        LoginError = string.Empty;

        // Валидация
        if (string.IsNullOrWhiteSpace(LoginModel.UserEmail))
        {
            LoginError = "Введите email";
            return;
        }

        if (string.IsNullOrWhiteSpace(LoginModel.Password))
        {
            LoginError = "Введите пароль";
            return;
        }

        try
        {
            IsLoading = true;
            StateHasChanged();

            LoginResponseModel loginResponse = await AuthService.LoginAsync(LoginModel);

            if (loginResponse.RequiresTwoFactor)
            {
                IsTwoFactorRequired = true;
                UserIdFor2FA = loginResponse.UserId;
                TwoFactorError = string.Empty;
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
                LoginError = "Неверный логин или пароль";
            }
        }
        catch (Exception ex)
        {
            LoginError = "Ошибка входа. Проверьте данные и попробуйте снова.";
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task VerifyTwoFactorAsync()
    {
        TwoFactorError = string.Empty;  // ← СБРОС

        if (string.IsNullOrWhiteSpace(TwoFactorCode))
        {
            TwoFactorError = "Введите код подтверждения";
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
                TwoFactorError = "Неверный код подтверждения";
                TwoFactorCode = string.Empty;
            }
        }
        catch (Exception ex)
        {
            TwoFactorError = "Ошибка проверки кода. Попробуйте снова.";
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
        TwoFactorError = string.Empty;  // ← СБРОС
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
}