namespace BlazorClient.Components.PagesComponents.MovieDetails;

public partial class MovieHeader : ComponentBase
{
    [Parameter, EditorRequired]
    public string Name { get; set; }

    [Parameter, EditorRequired]
    public string Description { get; set; }

    [Parameter, EditorRequired]
    public long Id { get; set; }

    [Parameter, EditorRequired]
    public int? ReleaseYear { get; set; }
}
