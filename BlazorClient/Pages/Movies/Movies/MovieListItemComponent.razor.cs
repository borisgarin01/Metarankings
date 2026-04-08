using Domain.Movies;

namespace BlazorClient.Pages.Movies.Movies;

public partial class MovieListItemComponent : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }

    [Parameter, EditorRequired]
    public Movie Movie { get; set; }
}
