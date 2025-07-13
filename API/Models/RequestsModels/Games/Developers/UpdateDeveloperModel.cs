namespace API.Models.RequestsModels.Games.Developers;

public sealed record UpdateDeveloperModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
