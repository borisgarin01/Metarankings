using BlazorClient.Auth;

namespace BlazorClient.Pages.Auth;

public partial class Logout : ComponentBase
{
    [Inject]
    public IAuthService AuthService { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                await AuthService.LogoutAsync();
                NavigationManager.NavigateTo("/", forceLoad: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\t{ex.StackTrace}");
            }
        }
    }
}
