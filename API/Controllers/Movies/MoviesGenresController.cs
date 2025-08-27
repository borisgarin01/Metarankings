using Data.Repositories.Classes.Derived.Movies;
using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/[controller]")]
public class MoviesGenresController : ControllerBase
{
    private readonly IRepository<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel> _moviesGenresRepository;

    public MoviesGenresController(IRepository<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel> moviesGenresRepository)
    {
        _moviesGenresRepository = moviesGenresRepository;
    }

    [HttpGet]
    public async Task<ActionResult<MovieGenre>> GetAllAsync()
    {
        try
        {
            var moviesDirectors = await _moviesGenresRepository.GetAllAsync();
            return Ok(moviesDirectors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<MovieGenre>> GetAsync(long id)
    {
        try
        {
            var movieDirector = await _moviesGenresRepository.GetAsync(id);
            if (movieDirector is null)
            {
                return NotFound();
            }
            return Ok(movieDirector);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<MovieGenre>> AddAsync(AddMovieGenreModel addMovieGenreModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var insertedId = await _moviesGenresRepository.AddAsync(addMovieGenreModel);

                var insertedMovieGenre = await _moviesGenresRepository.GetAsync(insertedId);

                return Created($"/api/moviesGenres/{insertedId}", insertedMovieGenre);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        return BadRequest();
    }
}

