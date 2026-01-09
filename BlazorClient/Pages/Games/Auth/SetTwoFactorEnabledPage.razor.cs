using BlazorClient.Auth;
using Domain.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorClient.Pages.Games.Auth;

[Authorize]
public partial class SetTwoFactorEnabledPage : ComponentBase
{
    private bool _twoFactorEnabled;
    private bool _isInitializing = true;

    public bool TwoFactorEnabled
    {
        get => _twoFactorEnabled;
        set
        {
            // Don't trigger change during initialization
            if (_twoFactorEnabled != value && !_isInitializing)
            {
                _twoFactorEnabled = value;
                _ = CheckboxChanged(value); // Fire and forget, or handle differently
            }
        }
    }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public IAuthService AuthService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Using CascadingParameter approach
        var authState = await AuthenticationStateTask;
        var user = authState.User;

        var claim = user.FindFirst("TwoFactorEnabled");
        if (claim != null && bool.TryParse(claim.Value, out var enabled))
        {
            // Set the private field directly without triggering the property setter
            _twoFactorEnabled = enabled;
        }

        _isInitializing = false;
    }

    private async Task CheckboxChanged(bool newValue)
    {
        try
        {
            // Update on the server
            await AuthService.SendTwoFactorEnabledMessage(new SetTwoFactorEnabledModel(newValue));

            await AuthService.LogoutAsync();

            await JSRuntime.InvokeVoidAsync("alert", "Настройки обновлены!");
        }
        catch (Exception ex)
        {
            // Revert UI state if update fails
            _twoFactorEnabled = !newValue;
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("alert", $"Ошибка: {ex.Message}");
        }
    }
}