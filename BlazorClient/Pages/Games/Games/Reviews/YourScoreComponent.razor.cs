using Domain.RequestsModels.Games.GamesGamersReviews;

namespace BlazorClient.Pages.Games.Games.Reviews;

public partial class YourScoreComponent : Components.PagesComponents.Common.YourScoreComponent
{
    public override async Task AddReviewAsync()
    {
        var addGamePlayerReviewModel = new AddGamePlayerReviewModel(GameId, Text, YourScore);
        HttpResponseMessage addingGamePlayerReviewHttpResponseMessage = await HttpClient.PostAsJsonAsync<AddGamePlayerReviewModel>("api/GamesGamersReviews", addGamePlayerReviewModel);
        if (addingGamePlayerReviewHttpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo($"/games/Details/{GameId}", true);
        else
            await JSRuntime.InvokeVoidAsync("alert", await addingGamePlayerReviewHttpResponseMessage.Content.ReadAsStringAsync());
    }
}
