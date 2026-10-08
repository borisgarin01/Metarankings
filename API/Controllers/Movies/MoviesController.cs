using API.Controllers.Games;
using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Classes.Derived.Movies;
using Data.Repositories.Interfaces.Derived;
using Domain.Movies;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Movies;
using Domain.RequestsModels.Movies.Movies;
using Domain.ResponsesModels;

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
        IEnumerable<Movie> moviesModels = await _moviesModelsRepository.GetAllAsync(cancellationToken);
        return Ok(moviesModels);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<IEnumerable<Movie>>> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        Movie? movieModel = await _moviesModelsRepository.GetAsync(id, cancellationToken);

        if (movieModel is null)
            return NotFound();

        return Ok(movieModel);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<long>> AddAsync(AddMovieModel movieModel, CancellationToken cancellationToken = default)
    {
        long insertedMovie = await _moviesModelsRepository.AddAsync(movieModel, cancellationToken);
        return Ok(insertedMovie);
    }

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<Movie>> UpdateAsync(long id, UpdateMovieModel updateMovieModel, CancellationToken cancellationToken = default)
    {
        try
        {
            Movie? updatedMovie = await _moviesModelsRepository.UpdateAsync(updateMovieModel, id, cancellationToken);

            if (updatedMovie is null)
                return NotFound();

            return Ok(updatedMovie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}\t{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{dateFrom:datetime}/{dateTo:datetime}")]
    public async Task<ActionResult<IEnumerable<Movie>>> GetAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default)
    {
        IEnumerable<Movie> movies = await _moviesModelsRepository.GetAsync(dateFrom, dateTo, cancellationToken);
        return Ok(movies);
    }

    [HttpGet("{pageNumber:long}/{pageSize:long}")]
    public async Task<ActionResult<IEnumerable<Movie>>> GetAsync(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
    {
        IEnumerable<Movie> movies = await _moviesModelsRepository.GetAsync((pageNumber - 1) * pageSize, pageSize, cancellationToken);
        return Ok(movies);
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<long>> RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        Movie movie = await _moviesModelsRepository.GetAsync(id, cancellationToken);
        if (movie is null)
            return NotFound();
        try
        {
            await _moviesModelsRepository.RemoveAsync(id, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}\t{ex.StackTrace}");
            return StatusCode(500, ex);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Movie>>> Search([FromQuery] string name, CancellationToken cancellationToken = default)
    {
        return Ok(await _moviesModelsRepository.GetByNameAsync(name, cancellationToken));
    }

    [HttpGet("most-waiting/{offset:int}/{limit:int}")]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMostWaitingAsync(
        int offset,
        int limit,
        [FromQuery] long[]? genresIds = null,
        CancellationToken cancellationToken = default)
    {
        if (offset < 0 || limit <= 0)
            return BadRequest();

        return Ok(await _moviesModelsRepository.GetMostWaitingAsync(genresIds, offset, limit, cancellationToken));
    }

    [HttpGet("most-waiting/count")]
    public async Task<ActionResult<int>> GetMostWaitingCountAsync([FromQuery] long[]? genresIds = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _moviesModelsRepository.GetMostWaitingCountAsync(genresIds, cancellationToken));
    }

    [HttpPost("byParameters")]
    public async Task<IActionResult> GetByParameters([FromBody] MovieFilterRequest filter, CancellationToken cancellationToken = default)
    {
        var movies = await _moviesModelsRepository.GetByParametersAsync(filter, cancellationToken);

        int totalCount = await _moviesModelsRepository.GetCountByParametersAsync(filter, cancellationToken);

        return Ok(new PagedResponse<Movie>
        {
            Items = movies,
            TotalCount = totalCount,
            Page = (filter.Skip / filter.Take) + 1,
            PageSize = filter.Take
        });
    }
}
