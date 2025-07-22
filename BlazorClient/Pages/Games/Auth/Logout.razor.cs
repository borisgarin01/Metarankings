
using BlazorClient.Auth;
using IdentityLibrary.Models;

namespace BlazorClient.Pages.Games.Auth;

public partial class Logout : ComponentBase
{
    [Inject]
    public IAuthService AuthService { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await AuthService.LogoutAsync();
            NavigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
             //Log exception
        }
    }
}
