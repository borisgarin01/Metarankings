using BlazorClient.PagesModels.Games.Reviews;
using Blazored.Toast.Services;
using Domain.RequestsModels.Games.GamesGamersReviews;

namespace BlazorClient.Pages.Games.Games.Reviews;

public partial class YourScoreComponent : ComponentBase
{
    private ClaimsPrincipal currentUser;

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }

    private YourScoreComponentModel YourScoreComponentModel { get; } = new();

    [Parameter, EditorRequired]
    public long GameId { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (AuthenticationState is not null)
        {
            AuthenticationState authState = await AuthenticationState;
            currentUser = authState?.User;
        }
        StateHasChanged();
    }

    public async Task AddReviewAsync()
    {
        AddGamePlayerReviewModel addGamePlayerReviewModel = new AddGamePlayerReviewModel(GameId, YourScoreComponentModel.Text, YourScoreComponentModel.YourScore);
        HttpResponseMessage addingGamePlayerReviewHttpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync<AddGamePlayerReviewModel>("/api/Games/GamesGamersReviews", addGamePlayerReviewModel);
        if (addingGamePlayerReviewHttpResponseMessage.IsSuccessStatusCode)
        {
            ToastService.ShowSuccess("Обзор добавлен");
            NavigationManager.NavigateTo($"/games/Details/{GameId}", true);
        }
        else
            ToastService.ShowError(await addingGamePlayerReviewHttpResponseMessage.Content.ReadAsStringAsync());
    }
}
