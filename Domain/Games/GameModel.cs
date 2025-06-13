using System.Text.Json.Serialization;

namespace Domain.Games;

public sealed record GameModel
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("image")]
    public required string Image { get; set; }

    [JsonPropertyName("score")]
    public float? Score { get; set; }

    [JsonPropertyName("scoresCount")]
    public long? ScoresCount { get; set; }

    [JsonPropertyName("developers")]
    public List<Developer> Developers { get; set; } = new();

    [JsonPropertyName("publisher")]
    public required Publisher Publisher { get; set; }

    [JsonPropertyName("genres")]
    public List<Genre> Genres { get; set; } = new();

    [JsonPropertyName("localization")]
    public required Localization Localization { get; set; }

    [JsonPropertyName("releaseDate")]
    public DateTime? ReleaseDate { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("trailer")]
    public string Trailer { get; set; }

    [JsonPropertyName("platforms")]
    public List<Platform> Platforms { get; set; } = new();

    [JsonPropertyName("screenshots")]
    public List<GameScreenshot> Screenshots { get; set; } = new();

    [JsonPropertyName("tags")]
    public List<Tag> Tags { get; set; } = new();
}
