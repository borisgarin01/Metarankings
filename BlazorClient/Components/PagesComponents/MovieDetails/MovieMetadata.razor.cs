using Domain.Games;
using Domain.Movies;

namespace BlazorClient.Components.PagesComponents.MovieDetails;

public partial class MovieMetadata : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Genre> Genres { get; set; }

    [Parameter, EditorRequired]
    public IEnumerable<MovieStudio> MoviesStudios { get; set; }

    [Parameter, EditorRequired]
    public DateTime PremierDate { get; set; }
}
