using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesStudios;

namespace BlazorClient.Pages.Admin.Movies.MoviesStudios;

public partial class RemoveMovieStudioPage : RemoveEntityPageBase<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>
{
    protected override string ListUrl => "/admin/movies/movies-studios/movies-studios-list";
}
