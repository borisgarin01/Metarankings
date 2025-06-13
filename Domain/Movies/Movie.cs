using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Movies;

[Table("Movies")]
public sealed record Movie
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("href")]
    public required string Href { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("originalName")]
    public required string OriginalName { get; set; }

    [JsonPropertyName("image")]
    public required string Image { get; set; }

    [JsonPropertyName("score")]
    public float? Score { get; set; }

    [JsonPropertyName("scoresCount")]
    public long? ScoresCount { get; set; }

    [JsonPropertyName("premierDate")]
    public DateTime? PremierDate { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("movieGenres")]
    public IEnumerable<MovieGenre> MovieGenres { get; set; } = Enumerable.Empty<MovieGenre>();

    [JsonPropertyName("moviesStudios")]
    public IEnumerable<MovieStudio> MoviesStudios { get; set; } = Enumerable.Empty<MovieStudio>();

    [JsonPropertyName("moviesDirectors")]
    public IEnumerable<MovieDirector> MoviesDirectors { get; set; } = Enumerable.Empty<MovieDirector>();
}
