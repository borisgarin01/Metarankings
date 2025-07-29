namespace Domain.Games;

[Table("platforms")]
public sealed record Platform
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    public List<Game> Games { get; set; } = new List<Game>();
}
