using Blazored.Toast.Services;
using Domain.Games;
using Domain.RequestsModels.Games.GamesGamersReviews;

namespace BlazorClient.Pages.Games.Games;

public partial class GameDetails : ComponentBase
{
    private ClaimsPrincipal currentUser;

    private bool isAbleToWriteComments = false;

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }
    public Game Game { get; set; }

    public float YourScore { get; set; }

    public string Text { get; set; }

    [Parameter, EditorRequired]
    public long Id { get; set; }


    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }
    public string TextContent { get; set; }
    public double Score { get; set; }

    public bool IsAbleToWriteComments
    {
        get => isAbleToWriteComments;
        private set
        {
            isAbleToWriteComments = value;
            StateHasChanged();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        Game = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Game>($"/api/Games/Games/{Id}");

        if (AuthenticationState is not null)
        {
            AuthenticationState authState = await AuthenticationState;
            currentUser = authState?.User;

            if (currentUser is not null
                && currentUser.Claims.FirstOrDefault(cuc => cuc.Type == ClaimTypes.NameIdentifier) != null)
            {
                long userId = long.Parse(currentUser.Claims.FirstOrDefault(cuc => cuc.Type == ClaimTypes.NameIdentifier).Value);
                if (Game.GamesPlayersReviews.FirstOrDefault(g => g.UserId == userId) is null)
                {
                    IsAbleToWriteComments = true;
                }
            }
        }
        StateHasChanged();
    }

    public async Task AddReviewAsync()
    {
        AddGamePlayerReviewModel addGamePlayerReviewModel = new AddGamePlayerReviewModel(Id, Text, YourScore);
        HttpResponseMessage addingGamePlayerReviewHttpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync<AddGamePlayerReviewModel>("/api/GamesGamersReviews", addGamePlayerReviewModel);
        if (!addingGamePlayerReviewHttpResponseMessage.IsSuccessStatusCode)
            ToastService.ShowError(await addingGamePlayerReviewHttpResponseMessage.Content.ReadAsStringAsync());
        else
            NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    // В основном компоненте страницы
    private async Task RefreshGameData()
    {
        Game = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Game>($"/api/Games/Games/{Id}");
        StateHasChanged();
    }
}