using System.Text.Json.Serialization;

namespace API.Models.RequestsModels.Genres;

public sealed record UpdateGenreModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
