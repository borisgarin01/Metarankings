using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;

namespace API.Controllers.Movies;

[Route("api/movies/[controller]")]
public sealed class GenresController : CrudControllerBase<Genre, AddMovieGenreModel, UpdateMovieGenreModel>
{
    public GenresController(IRepository<Genre, AddMovieGenreModel, UpdateMovieGenreModel> moviesGenresRepository) : base(moviesGenresRepository)
    {
    }
}
