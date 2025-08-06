namespace Domain.Games;

public sealed record GameComment
    ([property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("gameId")] long GameId,
    [property: JsonPropertyName("userId")] long UserId,
    [property: JsonPropertyName("textContent")] string TextContent);