using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;

namespace WebManagers.Derived.Movies;

public sealed class MoviesGenresWebManager : CrudWebManager<Genre, AddMovieGenreModel, UpdateMovieGenreModel>
{
    public MoviesGenresWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/movies/genres")
    {
    }
}
