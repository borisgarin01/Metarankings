namespace BlazorClient.Components;

public partial class RedirectToLogin : ComponentBase
{
    [Inject] public NavigationManager Navigation { get; set; }

    protected override void OnInitialized()
    {
        Navigation.NavigateTo("/auth/login");
    }
}

