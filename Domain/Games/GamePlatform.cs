﻿namespace Domain.Games;

[Table("gamePlatforms")]
public sealed record GamePlatform
{
    [JsonPropertyName("gameId")]
    public long GameId { get; set; }

    [JsonPropertyName("platformId")]
    public long PlatformId { get; set; }
}
