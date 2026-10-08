using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesPersons;

namespace BlazorClient.Pages.Admin.Movies.MoviesPersons;

public partial class RemoveMoviePersonPage : RemoveEntityPageBase<MoviePerson, AddMoviePersonModel, UpdateMoviePersonModel>
{
    protected override string ListUrl => "/admin/movies/movies-persons/movies-persons-list";
}
