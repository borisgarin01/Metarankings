using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;

namespace API.Controllers.Movies;

[Route("api/movies/[controller]")]
public sealed class MoviesDirectorsController : CrudControllerBase<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel>
{
    public MoviesDirectorsController(IRepository<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel> moviesDirectorsRepository) : base(moviesDirectorsRepository)
    {
    }
}
