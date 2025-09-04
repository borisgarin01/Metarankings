namespace BlazorClient.PagesModels.Games.Reviews;

public class YourScoreComponentModel : ComponentBase
{
    [Parameter, EditorRequired]
    [Range(0.0, 10.0)]
    public double AverageGameGamersScore { get; set; }

    [Parameter, EditorRequired]
    [Range(0.0f, 10.0f)]
    public float YourScore { get; set; }

    [Parameter, EditorRequired]
    [Range(0, long.MaxValue)]
    public long ScoresCount { get; set; }

    [Parameter, EditorRequired]
    [MinLength(1, ErrorMessage = "Write a review")]
    [MaxLength(4000, ErrorMessage = "Review is too long")]
    [Required(ErrorMessage = "Review text is required")]
    public string Text { get; set; }
}
