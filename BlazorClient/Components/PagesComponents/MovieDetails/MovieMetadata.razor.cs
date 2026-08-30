using Domain.Games;
using Domain.Movies;

namespace BlazorClient.Components.PagesComponents.MovieDetails;

public partial class MovieMetadata : ComponentBase
{
    [Parameter]
    public string OriginalName { get; set; }

    [Parameter, EditorRequired]
    public IEnumerable<Domain.Movies.Genre> Genres { get; set; }

    [Parameter, EditorRequired]
    public IEnumerable<MovieStudio> MoviesStudios { get; set; }

    [Parameter, EditorRequired]
    public DateOnly PremierDate { get; set; }
}
