using System.Text.Json.Serialization;

namespace API.Models.RequestsModels.Developers;

public sealed record AddDeveloperModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
