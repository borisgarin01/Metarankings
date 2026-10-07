using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;

namespace WebManagers.Derived.Movies;

public sealed class MoviesDirectorsWebManager : CrudWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel>
{
    public MoviesDirectorsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/movies/moviesDirectors")
    {
    }
}
