namespace Domain.Games;

[Table("GamesPublishers")]
public sealed record GamePublisher(
    [property: JsonPropertyName("gameId")]
    long GameId,

    [property: JsonPropertyName("publisherId")]
    long PublisherId
    );