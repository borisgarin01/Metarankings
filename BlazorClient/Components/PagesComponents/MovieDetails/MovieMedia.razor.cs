namespace BlazorClient.Components.PagesComponents.MovieDetails;

public partial class MovieMedia : ComponentBase
{
    [Parameter, EditorRequired]
    public long MovieId { get; set; }

    [Parameter, EditorRequired]
    public string? Trailer { get; set; }

    [Parameter, EditorRequired]
    public string Name { get; set; }

    [Parameter, EditorRequired]
    public string Image { get; set; }

    [Parameter]
    public DateOnly? PremierDate { get; set; }
}
