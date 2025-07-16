namespace Domain.Games;

[Table("platforms")]
public sealed record Platform
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("games")]
    public IEnumerable<Game> Games { get; set; }
}