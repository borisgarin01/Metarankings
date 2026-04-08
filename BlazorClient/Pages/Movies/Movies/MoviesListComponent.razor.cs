using Domain.Movies;

namespace BlazorClient.Pages.Movies.Movies;

public partial class MoviesListComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Movie> Movies { get; set; }
}
