using System.Text.Json.Serialization;

namespace Domain;
public sealed record Platform
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
