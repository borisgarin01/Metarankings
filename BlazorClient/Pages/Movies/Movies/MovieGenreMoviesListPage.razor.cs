namespace BlazorClient.Pages.Movies.Movies;

public partial class MovieGenreMoviesListPage : ComponentBase
{
    [Parameter, EditorRequired]
    public long Id { get; set; }
}
