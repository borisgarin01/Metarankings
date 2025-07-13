namespace API.Models.RequestsModels.Games.Platforms;

public sealed record AddPlatformModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
