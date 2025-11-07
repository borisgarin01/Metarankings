using Data.Repositories.Classes.Derived.Movies;
using Data.Repositories.Interfaces;
using Domain.Movies;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/movies/[controller]")]
public sealed class MoviesStudiosController : ControllerBase
{
    private readonly IRepository<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel> _moviesStudiosRepository;

    public MoviesStudiosController(IRepository<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel> moviesStudiosRepository)
    {
        _moviesStudiosRepository = moviesStudiosRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieStudio>>> GetAllAsync()
    {
        var moviesStudios = await _moviesStudiosRepository.GetAllAsync();

        return Ok(moviesStudios);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<MovieStudio>> AddAsync(AddMovieStudioModel movieStudio)
    {
        long createdMovieStudioId = await _moviesStudiosRepository.AddAsync(movieStudio);

        MovieStudio createdMovieStudio = await _moviesStudiosRepository.GetAsync(createdMovieStudioId);

        return Created($"/api/movies/moviesStudios/{createdMovieStudioId}", createdMovieStudio);
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        MovieStudio movieStudio = await _moviesStudiosRepository.GetAsync(id);

        if (movieStudio is null)
            return NotFound();

        await _moviesStudiosRepository.RemoveAsync(id);
        return NoContent();
    }

    [HttpGet("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<MovieStudio>> GetAsync(long id)
    {
        MovieStudio movieStudio = await _moviesStudiosRepository.GetAsync(id);

        if (movieStudio is null)
            return NotFound();

        return Ok(movieStudio);
    }
}
