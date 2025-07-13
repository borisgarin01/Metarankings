namespace API.Models.RequestsModels.Games.Publishers;

public sealed record AddPublisherModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
