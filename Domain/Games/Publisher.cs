namespace Domain.Games;

[Table("Publishers")]
public sealed record Publisher
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("games")]
    public IEnumerable<Game> Games { get; set; }
}