using Domain.Movies;

namespace BlazorClient.Components.PagesComponents.MovieDetails;

public partial class GenresComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Genre> Genres { get; set; } = Enumerable.Empty<Genre>();
}
