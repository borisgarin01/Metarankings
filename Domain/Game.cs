using System.ComponentModel.DataAnnotations.Schema;
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
    public DateOnly? ReleaseDate { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("trailer")]
    public required string Trailer { get; set; }
}
