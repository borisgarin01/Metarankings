﻿namespace Domain.Games;

[Table("Developers")]
public sealed record Developer
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("games")]
    public List<Game> Games { get; init; } = new();
}