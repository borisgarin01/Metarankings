using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;

namespace BlazorClient.Pages.Admin.Movies.MoviesDirectors;

public partial class RemoveMovieDirectorPage : RemoveEntityPageBase<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel>
{
    protected override string ListUrl => "/admin/movies/movies-directors/movies-directors-list";
}
