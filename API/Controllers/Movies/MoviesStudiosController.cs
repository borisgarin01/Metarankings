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
        var createdMovieStudioId = await _moviesStudiosRepository.AddAsync(movieStudio);

        var createdMovieStudio = await _moviesStudiosRepository.GetAsync(createdMovieStudioId);

        return Created($"/api/movies/moviesStudios/{createdMovieStudioId}", createdMovieStudio);
    }
}
