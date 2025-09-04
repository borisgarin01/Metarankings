namespace Domain.RequestsModels.Games.GamesGamersReviews;

public sealed record UpdateGamePlayerReviewModel
{
    [Required(ErrorMessage = "Text should be set")]
    [MinLength(1, ErrorMessage = "Text should be not empty")]
    public string TextContent { get; set; }

    [Range(0.0f, 10.0f, ErrorMessage = "Score should be between 0 and 10")]
    [JsonPropertyName("score")]
    public double Score { get; set; }
}