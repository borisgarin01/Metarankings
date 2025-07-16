namespace Domain.Games;

[Table("Developers")]
public sealed record Developer(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("games")] IEnumerable<Game> Games);