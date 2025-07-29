namespace Domain.Games;

[Table("Localizations")]
public sealed record Localization
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("games")]
    public List<Game> Games { get; set; } = new List<Game>();
}
