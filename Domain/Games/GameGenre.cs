namespace Domain.Games;

[Table("GamesGenres")]
public sealed record GameGenre
{
    [JsonPropertyName("gameId")]
    public long GameId { get; set; }

    [JsonPropertyName("game")]
    public Game Game { get; set; }

    [JsonPropertyName("genreId")]
    public long GenreId { get; set; }

    [JsonPropertyName("genre")]
    public Genre Genre { get; set; }
}
