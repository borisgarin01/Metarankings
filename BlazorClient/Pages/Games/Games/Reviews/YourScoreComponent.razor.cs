using BlazorClient.PagesModels.Games.Reviews;
using Blazored.Toast.Services;
using Domain.RequestsModels.Games.GamesGamersReviews;

namespace BlazorClient.Pages.Games.Games.Reviews;

public partial class YourScoreComponent : CancellableComponentBase
{
    private YourScoreComponentModel YourScoreComponentModel { get; } = new();

    [Parameter, EditorRequired]
    public long GameId { get; set; }

    [Parameter]
    public double GameScore { get; set; }

    [Parameter]
    public long ScoresCount { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    private void SetScore(int score)
    {
        YourScoreComponentModel.YourScore = score;
        StateHasChanged();
    }

    public async Task AddReviewAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(YourScoreComponentModel.Text))
            {
                ToastService.ShowError("Review text is required");
                return;
            }

            var addGamePlayerReviewModel = new AddGamePlayerReviewModel(
                GameId,
                YourScoreComponentModel.Text,
                YourScoreComponentModel.YourScore
            );

            var response = await HttpClientFactory
                .CreateClient("AuthorizedClient")
                .PostAsJsonAsync("/api/Games/GamesGamersReviews", addGamePlayerReviewModel, DisposalToken);

            if (response.IsSuccessStatusCode)
            {
                ToastService.ShowSuccess("Review added successfully!");
                NavigationManager.NavigateTo($"/games/Details/{GameId}", true);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync(DisposalToken);
                ToastService.ShowError($"Failed to add review: {error}");
            }
        }
        catch (Exception ex) when (!DisposalToken.IsCancellationRequested)
        {
            ToastService.ShowError($"Error: {ex.Message}");
        }
    }
}