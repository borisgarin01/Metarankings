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

    [JsonPropertyName("usersScore")]
    public required float UsersScore { get; set; }

    [JsonPropertyName("usersReviewsCount")]
    public required int UsersReviewsCount { get; set; }

    [JsonPropertyName("criticsScore")]
    public required float CriticsScore { get; set; }

    [JsonPropertyName("criticsReviewsCount")]
    public required int CriticsReviewsCount { get; set; }

    [JsonPropertyName("premierDate")]
    public DateOnly? PremierDate { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("movieGenres")]
    public List<Genre> MovieGenres { get; set; } = new List<Genre>();

    [JsonPropertyName("moviesStudios")]
    public List<MovieStudio> MoviesStudios { get; set; } = new List<MovieStudio>();

    [JsonPropertyName("moviesDirectors")]
    public List<MovieDirector> MoviesDirectors { get; set; } = new List<MovieDirector>();

    [JsonPropertyName("movieReviews")]
    public List<MovieViewerReview> MovieReviews { get; set; } = new List<MovieViewerReview>();
}
