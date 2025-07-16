namespace Domain.Games;

[Table("Games")]
public sealed record Game(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("image")] string Image,
    [property: JsonPropertyName("publisherId")] long PublisherId,
    [property: JsonPropertyName("publisher")] Publisher Publisher,
    [property: JsonPropertyName("releaseDate")] DateTime? ReleaseDate,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("trailer")] string Trailer,
    [property: JsonPropertyName("platforms")] IEnumerable<Platform> Platforms,
    [property: JsonPropertyName("genres")] IEnumerable<Genre> Genres,
    [property: JsonPropertyName("developers")] IEnumerable<Developer> Developers,
    [property: JsonPropertyName("gameScreenshots")] IEnumerable<GameScreenshot> Screenshots,
    [property: JsonPropertyName("localization")] Localization Localization);
