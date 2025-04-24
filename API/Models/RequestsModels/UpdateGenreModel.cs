using System.Text.Json.Serialization;

namespace API.Models.RequestsModels;

public class UpdateGenreModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
