using BlazorClient.Auth;

namespace BlazorClient.Pages.Auth;

public partial class GoogleCallback : ComponentBase
{
    [Inject] public IAuthService AuthService { get; set; }
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string? Token { get; set; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string? Error { get; set; }

    private bool IsProcessing { get; set; } = true;
    private bool IsError { get; set; }
    private string? ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(Error))
            {
                IsError = true;
                ErrorMessage = Error;
                IsProcessing = false;
                return;
            }

            if (!string.IsNullOrEmpty(Token))
            {
                // Store the token
                await AuthService.StoreTokenAsync(Token);

                // Update authentication state
                ((JwtAuthenticationStateProvider)AuthenticationStateProvider).MarkUserAsAuthenticated(Token);

                // Redirect to home page after successful login
                NavigationManager.NavigateTo("/", true);
            }
            else
            {
                IsError = true;
                ErrorMessage = "No authentication token received";
                IsProcessing = false;
            }
        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorMessage = $"Authentication failed: {ex.Message}";
            IsProcessing = false;
        }
    }

    public void GoToLogin()
    {
        NavigationManager.NavigateTo("/login", forceLoad: true);
    }
}
