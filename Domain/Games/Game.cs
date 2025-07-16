namespace Domain.Games;

[Table("Games")]
public sealed record Game
{
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("image")] public string Image { get; set; }
    [JsonPropertyName("publisher")] public Publisher Publisher { get; set; }
    [JsonPropertyName("releaseDate")] public DateTime? ReleaseDate { get; set; }
    [JsonPropertyName("description")] public string Description { get; set; }
    [JsonPropertyName("trailer")] public string Trailer { get; set; }
    [JsonPropertyName("platforms")] public List<Platform> Platforms { get; set; } = new();
    [JsonPropertyName("genres")] public List<Genre> Genres { get; set; }
    [JsonPropertyName("developers")] public List<Developer> Developers { get; set; }
    [JsonPropertyName("gameScreenshots")] public List<GameScreenshot> Screenshots { get; set; }
    [JsonPropertyName("localization")] public Localization Localization { get; set; }
}