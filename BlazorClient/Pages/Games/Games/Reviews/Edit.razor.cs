using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.Reviews;

namespace BlazorClient.Pages.Games.Games.Reviews;

public partial class Edit : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public long GameId { get; set; }

    public required string TextContent { get; set; }

    public required float Score { get; set; }

    private UpdateGamePlayerReviewModel UpdateGamePlayerReviewModel { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var gameReview = await HttpClient.GetFromJsonAsync<GameReview>(@$"/api/Games/GamesGamersReviews/{Id}");
        GameId = gameReview.GameId;
        TextContent = gameReview.TextContent;
        Score = gameReview.Score;
        UpdateGamePlayerReviewModel = new UpdateGamePlayerReviewModel
        {
            Score = gameReview.Score,
            TextContent = gameReview.TextContent
        };
        StateHasChanged();
    }

    public async Task UpdateAsync()
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync<UpdateGamePlayerReviewModel>($"/api/Games/gamesGamersReviews/{Id}",
            UpdateGamePlayerReviewModel);

        if (httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo($"/games/Details/{GameId}", true);

        else
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
