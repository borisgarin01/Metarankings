namespace Domain.RequestsModels.Movies.MoviesViewersReviews;

public sealed record UpdateMovieViewerReviewModel
{
    [Required(ErrorMessage = "Text should be set")]
    [MinLength(1, ErrorMessage = "Text should be not empty")]
    [MaxLength(10000, ErrorMessage = "Text is too long")]
    public string TextContent { get; set; }

    [Range(0.0f, 10.0f, ErrorMessage = "Score should be between 0 and 10")]
    [JsonPropertyName("score")]
    public float Score { get; set; }
}
