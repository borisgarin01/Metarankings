namespace BlazorClient.PagesModels.Games.Reviews;

public sealed class YourScoreComponentModel
{
    [Range(0, 10, ErrorMessage = "Score must be between 0 and 10")]
    public int YourScore { get; set; } = 5;

    [MinLength(1, ErrorMessage = "Write a review")]
    [MaxLength(4000, ErrorMessage = "Review is too long")]
    [Required(ErrorMessage = "Review text is required")]
    public string Text { get; set; } = string.Empty;
}