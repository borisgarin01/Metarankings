using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesPersons;

namespace API.Controllers.Movies;

[Route("api/movies/[controller]")]
public sealed class MoviesPersonsController : CrudControllerBase<MoviePerson, AddMoviePersonModel, UpdateMoviePersonModel>
{
    public MoviesPersonsController(IRepository<MoviePerson, AddMoviePersonModel, UpdateMoviePersonModel> moviesPersonsRepository) : base(moviesPersonsRepository)
    {
    }
}
