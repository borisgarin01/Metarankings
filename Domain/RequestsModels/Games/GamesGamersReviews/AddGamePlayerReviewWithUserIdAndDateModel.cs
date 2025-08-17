namespace Domain.RequestsModels.Games.GamesGamersReviews;

public sealed record AddGamePlayerReviewWithUserIdAndDateModel(
    [property: JsonPropertyName("gameId")]
    long GameId,

    [property: JsonPropertyName("text")]
    [Required(ErrorMessage = "Text is required")]
    [MaxLength(10000, ErrorMessage = "Text is too long")]
    [MinLength(1, ErrorMessage = "Text should be not empty")]
    string TextContent,

    [Range(0.0f, 10.0f, ErrorMessage = "Score should be between 0 and 10")]
    float Score,

    [property:JsonPropertyName("userId")]
    [Required(ErrorMessage = "User id is required")]
    long UserId,

    [property:JsonPropertyName("timeStamp")]
    [Required(ErrorMessage = "TimeStamp is required")]
    DateTime TimeStamp);