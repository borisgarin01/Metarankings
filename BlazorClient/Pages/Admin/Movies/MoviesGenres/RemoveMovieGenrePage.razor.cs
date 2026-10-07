using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;

namespace BlazorClient.Pages.Admin.Movies.MoviesGenres;

public partial class RemoveMovieGenrePage : RemoveEntityPageBase<Genre, AddMovieGenreModel, UpdateMovieGenreModel>
{
    protected override string ListUrl => "/admin/movies/movies-genres/movies-genres-list";
}
