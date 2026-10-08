using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesPersons;

namespace WebManagers.Derived.Movies;

public sealed class MoviesPersonsWebManager : CrudWebManager<MoviePerson, AddMoviePersonModel, UpdateMoviePersonModel>
{
    public MoviesPersonsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Movies/MoviesPersons")
    {
    }
}
