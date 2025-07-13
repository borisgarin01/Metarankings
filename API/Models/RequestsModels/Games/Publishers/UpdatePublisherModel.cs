namespace API.Models.RequestsModels.Games.Publishers;

public sealed record UpdatePublisherModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
