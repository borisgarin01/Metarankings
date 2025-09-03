namespace Domain.RequestsModels.Movies.MoviesViewersReviews;

public sealed record AddMovieViewerReviewModel(
    [property: JsonPropertyName("movieId")]
    long MovieId,

    [property: JsonPropertyName("text")]
    [Required(ErrorMessage = "Text is required")]
    [MaxLength(10000, ErrorMessage = "Text is too long")]
    [MinLength(1, ErrorMessage = "Text should be not empty")]
    string TextContent,

    [Range(0.0f, 10.0f, ErrorMessage = "Score should be between 0 and 10")]
    float Score);