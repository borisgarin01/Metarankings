using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesCountries;

namespace WebManagers.Derived.Movies;

public sealed class MoviesCountriesWebManager : CrudWebManager<MovieCountry, AddMovieCountryModel, UpdateMovieCountryModel>
{
    public MoviesCountriesWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Movies/MoviesCountries")
    {
    }
}
