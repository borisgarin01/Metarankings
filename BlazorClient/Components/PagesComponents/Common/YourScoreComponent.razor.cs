using Blazored.Toast.Services;

namespace BlazorClient.Components.PagesComponents.Common;

public abstract partial class YourScoreComponent : ComponentBase
{
    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }

    private ClaimsPrincipal currentUser;

    protected override async Task OnParametersSetAsync()
    {
        if (AuthenticationState is not null)
        {
            AuthenticationState authState = await AuthenticationState;
            currentUser = authState?.User;
        }
        StateHasChanged();
    }

    [Parameter, EditorRequired]
    [Range(0.0, 10.0)]
    public double AverageGameGamersScore { get; set; }

    [Parameter, EditorRequired]
    [Range(0.0f, 10.0f)]
    public float YourScore { get; set; }

    [Parameter, EditorRequired]
    [Range(0, long.MaxValue)]
    public long ScoresCount { get; set; }

    [Parameter, EditorRequired]
    [MinLength(1, ErrorMessage = "Write a review")]
    [MaxLength(4000, ErrorMessage = "Review is too long")]
    [Required(ErrorMessage = "Review text is required")]
    public string Text { get; set; }

    [Parameter, EditorRequired]
    public long GameId { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    public abstract Task AddReviewAsync();
}
