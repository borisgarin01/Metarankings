namespace Domain.Games;
public sealed record class DeveloperModel(
    [property: JsonPropertyName("id")] decimal Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("games")] IEnumerable<Game> Games,
    [property: JsonPropertyName("platforms")] IEnumerable<Platform> Platforms,
    [property: JsonPropertyName("genres")] IEnumerable<Genre> Genres
);
