using Data.Repositories.Interfaces;
using Domain.Movies;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/[controller]")]
public sealed class MoviesStudiosController : ControllerBase
{
    private readonly IRepository<MovieStudio> _moviesStudiosRepository;

    public MoviesStudiosController(IRepository<MovieStudio> moviesStudiosRepository)
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
    public async Task<ActionResult<MovieStudio>> AddAsync(MovieStudio movieStudio)
    {
        var createdMovieStudioId = await _moviesStudiosRepository.AddAsync(movieStudio);

        var createdMovieStudio = await _moviesStudiosRepository.GetAsync(createdMovieStudioId);

        return Ok(createdMovieStudio);
    }
}
