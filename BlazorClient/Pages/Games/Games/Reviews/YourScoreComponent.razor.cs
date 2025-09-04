using BlazorClient.PagesModels.Games.Reviews;
using Domain.RequestsModels.Games.GamesGamersReviews;

namespace BlazorClient.Pages.Games.Games.Reviews;

public partial class YourScoreComponent : ComponentBase
{
    [CascadingParameter]
    private Task<AuthenticationState>? authenticationState { get; set; }

    private ClaimsPrincipal currentUser;

    private YourScoreComponentModel YourScoreComponentModel { get; } = new();

    protected override async Task OnParametersSetAsync()
    {
        if (authenticationState is not null)
        {
            AuthenticationState authState = await authenticationState;
            currentUser = authState?.User;
        }
        StateHasChanged();
    }


    [Parameter, EditorRequired]
    public long GameId { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public async Task AddReviewAsync()
    {
        var addGamePlayerReviewModel = new AddGamePlayerReviewModel(GameId, YourScoreComponentModel.Text, YourScoreComponentModel.YourScore);
        HttpResponseMessage addingGamePlayerReviewHttpResponseMessage = await HttpClient.PostAsJsonAsync<AddGamePlayerReviewModel>("api/Games/GamesGamersReviews", addGamePlayerReviewModel);
        if (addingGamePlayerReviewHttpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo($"/games/Details/{GameId}", true);
        else
            await JSRuntime.InvokeVoidAsync("alert", await addingGamePlayerReviewHttpResponseMessage.Content.ReadAsStringAsync());
    }
}
