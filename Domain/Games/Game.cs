namespace Domain.Games;

[Table("Games")]
public sealed record Game
{
    [JsonPropertyName("id")]
    public required long Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("image")]
    public required string Image { get; set; }

    [JsonPropertyName("publisherId")]
    public required long PublisherId { get; set; }
    [JsonPropertyName("publisher")]
    public required Publisher Publisher { get; set; }

    [JsonPropertyName("releaseDate")]
    public required DateTime? ReleaseDate { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("trailer")]
    public required string Trailer { get; set; }

    [JsonPropertyName("platforms")]
    public required List<Platform> Platforms { get; set; } = new();

    [JsonPropertyName("genres")]
    public required List<Genre> Genres { get; set; } = new();

    [JsonPropertyName("developers")]
    public required List<Developer> Developers { get; set; } = new();
    
    [JsonPropertyName("gameScreenshots")]
    public required List<GameScreenshot> Screenshots { get; set; } = new();

    [JsonPropertyName("localization")]
    public required Localization Localization { get; set; }
}
