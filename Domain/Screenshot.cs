using System.Text.Json.Serialization;

namespace Domain;

public class Screenshot
{
    [JsonPropertyName("small")]
    public string Small { get; set; }

    [JsonPropertyName("original")]
    public string Original { get; set; }
}