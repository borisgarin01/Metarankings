namespace API.Models.ResponsesModels;

public sealed record DeveloperResponseModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
