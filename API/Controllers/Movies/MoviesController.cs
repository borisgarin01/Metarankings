using API.Controllers.Games;
using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Classes.Derived.Movies;
using Data.Repositories.Interfaces.Derived;
using Domain.Movies;
using Domain.RequestsModels.Movies.Movies;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/[controller]")]
public sealed class MoviesController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMoviesRepository _moviesModelsRepository;
    private readonly ILogger<MoviesController> _logger;

    private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    public MoviesController(IConfiguration configuration, IMoviesRepository moviesModelsRepository, ILogger<MoviesController> logger)
    {
        _configuration = configuration;
        _moviesModelsRepository = moviesModelsRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movie>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Movie> moviesModels = await _moviesModelsRepository.GetAllAsync();
        return Ok(moviesModels);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<IEnumerable<Movie>>> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        Movie? movieModel = await _moviesModelsRepository.GetAsync(id);

        if (movieModel is null)
            return NotFound();

        return Ok(movieModel);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<long>> AddAsync(AddMovieModel movieModel)
    {
        long insertedMovie = await _moviesModelsRepository.AddAsync(movieModel);
        return Ok(insertedMovie);
    }

    [HttpGet("{dateFrom:datetime}/{dateTo:datetime}")]
    public async Task<ActionResult<IEnumerable<Movie>>> GetAsync(DateTime dateFrom, DateTime dateTo)
    {
        IEnumerable<Movie> movies = await _moviesModelsRepository.GetAsync(dateFrom, dateTo);
        return Ok(movies);
    }

    [HttpGet("{pageNumber:long}/{pageSize:long}")]
    public async Task<ActionResult<IEnumerable<Movie>>> GetAsync(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
    {
        IEnumerable<Movie> movies = await _moviesModelsRepository.GetAsync((pageNumber - 1) * pageSize, pageSize);
        return Ok(movies);
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<long>> RemoveAsync(long id)
    {
        Movie movie = await _moviesModelsRepository.GetAsync(id);
        if (movie is null)
            return NotFound();
        try
        {
            await _moviesModelsRepository.RemoveAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}\t{ex.StackTrace}");
            return StatusCode(500, ex);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Movie>>> Search([FromQuery] string name)
    {
        return Ok(await _moviesModelsRepository.GetByNameAsync(name));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Movie>>> SearchByGenreId([FromQuery] long genreId)
    {
        return Ok(await _moviesModelsRepository.GetByGenreAsync(genreId));
    }
}
