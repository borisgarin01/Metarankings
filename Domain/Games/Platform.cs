namespace Domain.Games;

[Table("platforms")]
public sealed record Platform(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("games")] IEnumerable<Game> Games);