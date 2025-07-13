namespace API.Models.RequestsModels.Games.Developers;

public sealed record AddDeveloperModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
