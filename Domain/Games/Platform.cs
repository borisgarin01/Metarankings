using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Games;

[Table("platforms")]
public sealed record Platform
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("href")]
    public string Href { get; set; }
    public IEnumerable<Game> Games { get; set; } = Enumerable.Empty<Game>();
}
