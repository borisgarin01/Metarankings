using System.Text.Json.Serialization;

namespace Domain;
public sealed record Game
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("href")]
    public required string Href { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("image")]
    public required string Image { get; set; }

    [JsonPropertyName("score")]
    public float? Score { get; set; }

    [JsonPropertyName("scoresCount")]
    public long? ScoresCount { get; set; }

    [JsonPropertyName("developers")]
    public required string[] Developers { get; set; }

    [JsonPropertyName("publisher")]
    public required string Publisher { get; set; }

    [JsonPropertyName("genres")]
    public required string[] Genres { get; set; }

    [JsonPropertyName("localization")]
    public required string Localization { get; set; }

    [JsonPropertyName("releaseDate")]
    public DateOnly? ReleaseDate { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("trailer")]
    public string Trailer { get; set; }

    [JsonPropertyName("platforms")]
    public Platform[] Platforms { get; set; } = Array.Empty<Platform>();

    [JsonPropertyName("screenshots")]
    public Screenshot[] Screenshots { get; set; } = Array.Empty<Screenshot>();
}
