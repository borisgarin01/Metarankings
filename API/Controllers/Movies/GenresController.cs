using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/movies/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IRepository<Genre, AddMovieGenreModel, UpdateMovieGenreModel> _moviesGenresRepository;

    public GenresController(IRepository<Genre, AddMovieGenreModel, UpdateMovieGenreModel> moviesGenresRepository)
    {
        _moviesGenresRepository = moviesGenresRepository;
    }

    [HttpGet]
    public async Task<ActionResult<Genre>> GetAllAsync()
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
    public async Task<ActionResult<Genre>> GetAsync(long id)
    {
        try
        {
            Genre? movieGenre = await _moviesGenresRepository.GetAsync(id);
            if (movieGenre is null)
            {
                return NotFound();
            }
            return Ok(movieGenre);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<Genre>> AddAsync(AddMovieGenreModel addMovieGenreModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var insertedId = await _moviesGenresRepository.AddAsync(addMovieGenreModel);

                var insertedMovieGenre = await _moviesGenresRepository.GetAsync(insertedId);

                return Created($"/api/movies/moviesGenres/{insertedId}", insertedMovieGenre);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        return BadRequest();
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<Genre>> DeleteAsync(long id)
    {
        try
        {
            Genre movieGenre = await _moviesGenresRepository.GetAsync(id);

            if (movieGenre is null)
                return NotFound();

            try
            {
                await _moviesGenresRepository.RemoveAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}

