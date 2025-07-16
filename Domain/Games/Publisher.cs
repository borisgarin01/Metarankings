namespace Domain.Games;

[Table("Publishers")]
public sealed record Publisher(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("games")] IEnumerable<Game> Games);