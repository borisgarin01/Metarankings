namespace Domain.RequestsModels.Games.GamesGamersReviews;

public sealed record UpdateGamePlayerReviewModel(
    [Required(ErrorMessage ="Text should be set")]
    [MinLength(1,ErrorMessage ="Text should be not empty")]
    string TextContent,

    [Range(0.0f, 10.0f, ErrorMessage ="Score should be between 0 and 10")]
    [property:JsonPropertyName("score")]
    double Score);