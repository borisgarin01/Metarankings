namespace BlazorClient.Pages;

public sealed partial class LatestReviewItemComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public string Href { get; set; }

    [Parameter, EditorRequired]
    public string Title { get; set; }
}
