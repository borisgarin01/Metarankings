namespace Domain.Games;

public sealed record Genre
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("games")]
    public IEnumerable<Game> Games { get; set; } = Enumerable.Empty<Game>();
}
