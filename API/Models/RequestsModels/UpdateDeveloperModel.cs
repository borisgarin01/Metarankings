using System.Text.Json.Serialization;

namespace API.Models.RequestsModels;

public sealed record UpdateDeveloperModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
