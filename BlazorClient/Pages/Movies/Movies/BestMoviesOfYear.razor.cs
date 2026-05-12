namespace BlazorClient.Pages.Movies.Movies;

public partial class BestMoviesOfYear : ComponentBase
{
    [Parameter, EditorRequired]
    public short Year { get; set; }
}
