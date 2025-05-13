using System.Text.Json.Serialization;

namespace API.Models.RequestsModels.Publishers;

public sealed record UpdatePublisherModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
