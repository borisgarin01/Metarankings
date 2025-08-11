using Domain.RequestsModels.Games.GamesGamersReviews;
using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Pages.Games.Games;

public partial class YourScoreComponent : ComponentBase
{
    [Parameter, EditorRequired]
    [Range(0.0, 10.0)]
    public double AverageGameGamersScore { get; set; }

    [Parameter, EditorRequired]
    [Range(0.0, 10.0)]
    public double YourScore { get; set; }

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
    public HttpClient HttpClient { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public async Task AddReviewAsync()
    {
        var addGamePlayerReviewModel = new AddGamePlayerReviewModel(GameId, Text, YourScore);
        HttpResponseMessage addingGamePlayerReviewHttpResponseMessage = await HttpClient.PostAsJsonAsync<AddGamePlayerReviewModel>("api/GamesGamersReviews", addGamePlayerReviewModel);
        await JSRuntime.InvokeVoidAsync("alert", addingGamePlayerReviewHttpResponseMessage.StatusCode);
    }
}
