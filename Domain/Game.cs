using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace Domain;

[Table("Games")]
public sealed record Game
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("href")]
    public required string Href { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("image")]
    public string Image { get; set; }

    [JsonPropertyName("localizationId")]
    public required long? LocalizationId { get; set; }
    [JsonPropertyName("publisherId")]
    public required long PublisherId { get; set; }

    [JsonPropertyName("releaseDate")]
    public DateTime? ReleaseDate { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("trailer")]
    public required string Trailer { get; set; }

    [JsonPropertyName("platforms")]
    public IEnumerable<Platform> Platforms { get; set; } = Enumerable.Empty<Platform>();

    [JsonPropertyName("genres")]
    public IEnumerable<Genre> Genres { get; set; } = Enumerable.Empty<Genre>();
}
