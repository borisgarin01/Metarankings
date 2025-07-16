using Data.Repositories.Interfaces;
using Domain.Movies;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/[controller]")]
public sealed class MoviesController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IRepository<MovieModel> _moviesModelsRepository;

    private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    public MoviesController(IConfiguration configuration, IRepository<MovieModel> moviesModelsRepository)
    {
        _configuration = configuration;
        _moviesModelsRepository = moviesModelsRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var moviesModels = await _moviesModelsRepository.GetAllAsync();
        return Ok(moviesModels);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<IEnumerable<MovieModel>>> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        var movieModel = await _moviesModelsRepository.GetAsync(id);

        if (movieModel is null)
            return NotFound();

        return Ok(movieModel);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<long>> AddAsync(MovieModel movieModel)
    {
        var insertedMovie = await _moviesModelsRepository.AddAsync(movieModel);
        return Ok(insertedMovie);
    }


}
