using System.Text.Json.Serialization;

namespace API.Models.RequestsModels.Games.Platforms;

public sealed record UpdatePlatformModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("href")]
    public string Href { get; set; }
}
