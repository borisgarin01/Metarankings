namespace Domain.Games;
public sealed record GameScreenshot
(
    [property: JsonPropertyName("id")]
    long Id,

    [property: Required]
    [property: JsonPropertyName("url")]
    string Url,

    [property: Required]
    [property: JsonPropertyName("gameId")]
    long GameId
);