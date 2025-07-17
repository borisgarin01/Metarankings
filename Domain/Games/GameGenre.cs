namespace Domain.Games;

[Table("GamesGenres")]
public sealed record GameGenre(
    [property:JsonPropertyName("id")]
    long Id,

    [property: JsonPropertyName("gameId")]
    long GameId,

    [property: JsonPropertyName("genreId")]
    long GenreId
    );