using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesCountries;

namespace API.Controllers.Movies;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Movies })]
[Route("api/movies/[controller]")]
public sealed class MoviesCountriesController : CrudControllerBase<MovieCountry, AddMovieCountryModel, UpdateMovieCountryModel>
{
    public MoviesCountriesController(IRepository<MovieCountry, AddMovieCountryModel, UpdateMovieCountryModel> moviesCountriesRepository) : base(moviesCountriesRepository)
    {
    }
}
