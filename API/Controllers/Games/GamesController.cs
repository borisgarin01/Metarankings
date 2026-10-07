using API.Json;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels;
using Domain.RequestsModels.Games;
using Domain.ResponsesModels;

namespace API.Controllers.Games;

[ApiController]
[Route("api/games/[controller]")]
public sealed class GamesController : ControllerBase
{
    private JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
    private readonly IGamesRepository _gamesRepository;
    private readonly ILogger<GamesController> _logger;

    public GamesController(IGamesRepository gamesRepository, ILogger<GamesController> logger)
    {
        jsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter("yyyy-MM-dd"));
        _gamesRepository = gamesRepository;
        _logger = logger;
    }

    [HttpGet("First/{offset:int}/{limit:int}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetFirstAsync(int offset = 0, int limit = 25, CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> games = await _gamesRepository.GetFirstAsync(offset, limit);
        return Ok(games);
    }

    [HttpGet("Last/{offset:int}/{limit:int}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetLastAsync(int offset = 0, int limit = 25, CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> games = await _gamesRepository.GetLastAsync(offset, limit);
        return Ok(games);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<long>> AddAsync(AddGameModel addGameModel)
    {
        long createdGameId = await _gamesRepository.AddAsync(addGameModel);

        Game createdGame = await _gamesRepository.GetAsync(createdGameId);

        return Created($"api/games/{createdGame.Id}", createdGame);
    }

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<Game>> UpdateAsync(long id, UpdateGameModel updateGameModel)
    {
        try
        {
            Game? updatedGame = await _gamesRepository.UpdateAsync(updateGameModel, id);

            if (updatedGame is null)
                return NotFound();

            return Ok(updatedGame);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}\t{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<long>> RemoveAsync(long id)
    {
        Game game = await _gamesRepository.GetAsync(id);
        if (game is null)
            return NotFound();
        try
        {
            await _gamesRepository.RemoveAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}\t{ex.StackTrace}");
            return StatusCode(500, ex);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> games = await _gamesRepository.GetAllAsync();
        return Ok(games);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Game>> GetAsync(long id)
    {
        try
        {
            Game? game = await _gamesRepository.GetAsync(id);

            if (game is null)
                return NotFound();

            return Ok(game);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message, ex.StackTrace);
            return StatusCode(500, new { ex.Message, ex.StackTrace });
        }
    }

    [HttpPost("byParameters")]
    public async Task<ActionResult<PagedResponse<Game>>> GetByParametersAsync(GameFilterRequest filter)
    {
        // Получаем общее количество
        int totalCount = await _gamesRepository.GetCountByParametersAsync(
            filter.GenresIds,
            filter.PlatformsIds,
            filter.Years,
            filter.DevelopersIds,
            filter.PublishersIds,
            filter.LocalizationIds
        );

        // Получаем элементы для текущей страницы
        IEnumerable<Game> games = await _gamesRepository.GetByParametersAsync(
            filter.GenresIds,
            filter.PlatformsIds,
            filter.Years,
            filter.DevelopersIds,
            filter.PublishersIds,
            filter.LocalizationIds,
            filter.Skip,
            filter.Take
        );

        int page = (filter.Skip / filter.Take) + 1;

        var response = new PagedResponse<Game>
        {
            Items = games,
            TotalCount = totalCount,
            Page = page,
            PageSize = filter.Take
        };

        return Ok(response);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Game>>> Search([FromQuery] string name)
    {
        return Ok(await _gamesRepository.GetByNameAsync(name));
    }

    [HttpGet("games-releases-dates/{offset}/{limit}")]
    public async Task<ActionResult<IEnumerable<Game>>> GamesReleasesDates(
    short offset,
    short limit,
    [FromQuery] long[]? genresIds = null,
    [FromQuery] long[]? platformsIds = null)
    {
        if (genresIds?.Length > 0 || platformsIds?.Length > 0)
        {
            return Ok(await _gamesRepository.GetNearestByParametersAsync(
                genresIds, platformsIds, offset, limit));
        }

        return Ok(await _gamesRepository.GetNearestAsync(offset, limit));
    }

    [HttpGet("games-releases-dates/count")]
    public async Task<ActionResult<int>> GamesReleasesDatesCount(
        [FromQuery] long[]? genresIds = null,
        [FromQuery] long[]? platformsIds = null)
    {
        return Ok(await _gamesRepository.GetNearestCountByParametersAsync(genresIds, platformsIds));
    }

    [HttpGet("most-waiting/{offset:int}/{limit:int}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetMostWaitingAsync(
        int offset,
        int limit,
        [FromQuery] long[]? genresIds = null,
        [FromQuery] long[]? platformsIds = null)
    {
        if (offset < 0 || limit <= 0)
            return BadRequest();

        return Ok(await _gamesRepository.GetMostWaitingAsync(genresIds, platformsIds, offset, limit));
    }

    [HttpGet("most-waiting/count")]
    public async Task<ActionResult<int>> GetMostWaitingCountAsync(
        [FromQuery] long[]? genresIds = null,
        [FromQuery] long[]? platformsIds = null)
    {
        return Ok(await _gamesRepository.GetMostWaitingCountAsync(genresIds, platformsIds));
    }
}
