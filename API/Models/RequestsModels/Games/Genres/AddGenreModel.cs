namespace API.Models.RequestsModels.Games.Genres;

public sealed record AddGenreModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
