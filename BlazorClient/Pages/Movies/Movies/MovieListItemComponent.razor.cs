using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;

namespace BlazorClient.Pages.Movies.Movies;

public partial class MovieListItemComponent : ComponentBase
{

    [Parameter, EditorRequired]
    public string Href { get; set; }

    [Parameter, EditorRequired]
    public string Title { get; set; }

    [Parameter, EditorRequired]
    public string ReviewsHref { get; set; }

    [Parameter, EditorRequired]
    public string ImageSource { get; set; }

    [Parameter, EditorRequired]
    public string ImageAlt { get; set; }

    [Parameter, EditorRequired]
    public DateTime ReleaseDate { get; set; }

    [Parameter, EditorRequired]
    public string Description { get; set; }

    [Parameter, EditorRequired]
    public IEnumerable<ListMovieGenreModel> ListMovieGenreModels { get; set; }
}
