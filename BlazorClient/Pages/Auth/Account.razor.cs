using BlazorClient.Auth;
using Blazored.Toast.Services;
using Domain.Auth;
using IdentityLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Auth;

[Authorize]
public partial class Account : ComponentBase
{
    private bool twoFactorEnabled;
    private string newPassword;
    private string? currentPassword;
    private bool isInitializing = true;

    public bool TwoFactorEnabled
    {
        get => twoFactorEnabled;
        set
        {
            // Don't trigger change during initialization
            if (twoFactorEnabled != value && !isInitializing)
            {
                twoFactorEnabled = value;
                _ = CheckboxChanged(value); // Fire and forget, or handle differently
            }
        }
    }

    public string? CurrentPassword
    {
        get => currentPassword;
        set
        {
            if (currentPassword != value)
            {
                currentPassword = value;
                StateHasChanged();
            }
        }
    }

    public string NewPassword
    {
        get => newPassword;
        set
        {
            if (newPassword != value)
            {
                newPassword = value;
                StateHasChanged();
            }
        }
    }

    public string NewPasswordConfirm
    {
        get => newPasswordConfirm;
        set
        {
            if (newPasswordConfirm != value)
            {
                newPasswordConfirm = value;
                StateHasChanged();
            }
        }
    }

    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public IAuthService AuthService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    private ApplicationUser applicationUser;
    private string newPasswordConfirm;

    public ApplicationUser ApplicationUser
    {
        get
        {
            return applicationUser;
        }
        private set
        {
            applicationUser = value;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        // Using CascadingParameter approach
        AuthenticationState authState = await AuthenticationStateTask;
        ClaimsPrincipal user = authState.User;

        Claim? claim = user.FindFirst("TwoFactorEnabled");
        if (claim != null && bool.TryParse(claim.Value, out bool enabled))
        {
            // Set the private field directly without triggering the property setter
            TwoFactorEnabled = enabled;
        }

        ApplicationUser = await AuthService.GetCurrentUserAsync();

        isInitializing = false;
    }

    private async Task CheckboxChanged(bool newValue)
    {
        try
        {
            // Update on the server
            await AuthService.SendTwoFactorEnabledMessage(new SetTwoFactorEnabledModel(newValue));

            await AuthService.LogoutAsync();
            ToastService.ShowError("Настройки обновлены!");
            StateHasChanged();
        }
        catch (Exception ex)
        {
            // Revert UI state if update fails
            TwoFactorEnabled = !newValue;
            StateHasChanged();
            ToastService.ShowError($"Ошибка: {ex.Message}");
        }
    }
    private async Task ChangePassword()
    {
        if (NewPassword != NewPasswordConfirm)
        {
            ToastService.ShowWarning("Пароли не совпадают!");
            return;
        }
        try
        {
            HttpResponseMessage httpResponseMessage = await AuthService.SendChangePasswordMessageAsync(new ChangePasswordModel(CurrentPassword, NewPassword));
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                await AuthService.LogoutAsync();
                StateHasChanged();
                ToastService.ShowSuccess("Пароль успешно изменён!");
            }
            else
            {
                ToastService.ShowError($"Ошибка: {await httpResponseMessage.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Ошибка: {ex.Message}{ex.StackTrace}");
        }
    }
}