using Domain.Movies;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Movies.Movies;

namespace BlazorClient.Pages.Admin.Movies.Movies;

public partial class RemoveMoviePage : RemoveEntityPageBase<Movie, AddMovieModel, UpdateMovieModel>
{
    protected override string ListUrl => "/admin/movies/list-movies";
}
