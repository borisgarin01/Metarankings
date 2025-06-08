using System.Text.Json.Serialization;

namespace Domain.Games;
public sealed record class DeveloperModel
{
    public decimal Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    [JsonPropertyName("games")]
    public required List<Game> Games { get; set; } = new();
    [JsonPropertyName("platforms")]
    public required List<Platform> Platforms { get; set; } = new();
    [JsonPropertyName("genres")]
    public required List<Genre> Genres { get; set; } = new();
}
