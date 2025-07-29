namespace Domain.Games;

[Table("Games")]
public sealed record Game
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("image")] public string Image { get; init; }
    [JsonPropertyName("publisher")] public Publisher Publisher { get; init; }
    [JsonPropertyName("releaseDate")] public DateTime? ReleaseDate { get; init; }
    [JsonPropertyName("description")] public string Description { get; init; }
    [JsonPropertyName("trailer")] public string Trailer { get; init; }
    [JsonPropertyName("platforms")] public List<Platform> Platforms { get; init; } = new();
    [JsonPropertyName("genres")] public List<Genre> Genres { get; init; } = new();
    [JsonPropertyName("developers")] public List<Developer> Developers { get; init; } = new();
    [JsonPropertyName("gameScreenshots")] public List<GameScreenshot> Screenshots { get; init; } = new();
    [JsonPropertyName("localization")] public Localization Localization { get; init; }
}
