namespace API.Models.RequestsModels.Games.Platforms;

public sealed record UpdatePlatformModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
