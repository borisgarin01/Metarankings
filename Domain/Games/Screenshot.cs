namespace Domain.Games;

public class Screenshot
{
    [JsonPropertyName("small")]
    public string Small { get; set; }

    [JsonPropertyName("original")]
    public string Original { get; set; }
}