namespace Domain.Games;
public sealed record class DeveloperModel
{
    public decimal Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    [JsonPropertyName("games")]
    public List<Game> Games { get; set; } = new();
    [JsonPropertyName("platforms")]
    public List<Platform> Platforms { get; set; } = new();
    [JsonPropertyName("genres")]
    public List<Genre> Genres { get; set; } = new();
}
