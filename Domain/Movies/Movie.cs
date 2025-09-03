using Domain.Reviews;

namespace Domain.Movies;

[Table("Movies")]
public sealed record Movie
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("originalName")]
    public required string OriginalName { get; set; }

    [JsonPropertyName("imageSource")]
    public required string ImageSource { get; set; }

    [JsonPropertyName("score")]
    public float? Score { get; set; }

    [JsonPropertyName("scoresCount")]
    public long? ScoresCount { get; set; }

    [JsonPropertyName("premierDate")]
    public DateTime? PremierDate { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("movieGenres")]
    public List<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();

    [JsonPropertyName("moviesStudios")]
    public List<MovieStudio> MoviesStudios { get; set; } = new List<MovieStudio>();

    [JsonPropertyName("moviesDirectors")]
    public List<MovieDirector> MoviesDirectors { get; set; } = new List<MovieDirector>();

    [JsonPropertyName("movieReviews")]
    public List<MovieReview> MovieReviews { get; set; } = new List<MovieReview>();
}
