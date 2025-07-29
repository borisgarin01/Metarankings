namespace Domain.Games;

[Table("gamePlatforms")]
public sealed record GamePlatform
(
    [property: JsonPropertyName("gameId")]
    long GameId,
    
    [property:JsonPropertyName("platformId")]
    long PlatformId);
