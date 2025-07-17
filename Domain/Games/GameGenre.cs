namespace Domain.Games;

[Table("GamesGenres")]
public sealed record GameGenre(
    [property: JsonPropertyName("gameId")]
    long GameId,

    [property: JsonPropertyName("game")]
    Game Game,

    [property: JsonPropertyName("genreId")]
    long GenreId,

    [property: JsonPropertyName("genre")]
    Genre Genre
    );