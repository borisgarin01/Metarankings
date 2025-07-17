namespace Domain.Games;

[Table("Localizations")]
public sealed record Localization
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("games")]
    public List<Game> Games { get; init; } = new();
}
