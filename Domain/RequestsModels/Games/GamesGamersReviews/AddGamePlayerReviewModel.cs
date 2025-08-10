namespace Domain.RequestsModels.Games.GamesGamersReviews;

public sealed record AddGamePlayerReviewModel(
    [property: JsonPropertyName("gameId")]
    long GameId,

    [property:JsonPropertyName("text")]
    [Required(ErrorMessage ="Text is required")]
    [MaxLength(10000, ErrorMessage ="Text is too long")]
    [MinLength(1, ErrorMessage ="Text should be not empty")]
    string TextContent,

    [Range(0.0f, 10.0f)]
    float Score);
