namespace Domain.Games;
public sealed record GameScreenshot
(
    [property: JsonPropertyName("id")]
    long Id,

    [property: Required]
    [property: JsonPropertyName("imageUrl")]
    string ImageUrl,

    [property: Required]
    [property: JsonPropertyName("gameId")]
    long GameId
);