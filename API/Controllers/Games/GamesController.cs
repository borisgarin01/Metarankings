using API.Json;
using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games;
using Domain.ResponsesModels;

namespace API.Controllers.Games;

[ApiController]
[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Games })]
[Route("api/games/[controller]")]
public sealed class GamesController : CrudControllerBase<Game, AddGameModel, UpdateGameModel>
{
    private readonly GamesRepository _gamesRepository;
    private readonly ILogger<GamesController> _logger;

    public GamesController(GamesRepository repository, ILogger<GamesController> logger) : base(repository)
    {
        _gamesRepository = repository;
        _logger = logger;
    }

    // =====================================================================
    //  УНИКАЛЬНЫЕ ЭНДПОИНТЫ (в CrudControllerBase их нет)
    // =====================================================================

    [HttpGet("First/{offset:int}/{limit:int}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetFirstAsync(
        int offset = 0,
        int limit = 25,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> games = await _gamesRepository.GetFirstAsync(offset, limit);
        return Ok(games);
    }

    [HttpGet("Last/{offset:int}/{limit:int}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetLastAsync(
        int offset = 0,
        int limit = 25,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> games = await _gamesRepository.GetLastAsync(offset, limit);
        return Ok(games);
    }

    [HttpPost("byParameters")]
    public async Task<ActionResult<PagedResponse<Game>>> GetByParametersAsync(GameFilterRequest filter)
    {
        int totalCount = await _gamesRepository.GetCountByParametersAsync(
            filter.GenresIds,
            filter.PlatformsIds,
            filter.Years,
            filter.DevelopersIds,
            filter.PublishersIds,
            filter.LocalizationIds);

        IEnumerable<Game> games = await _gamesRepository.GetByParametersAsync(
            filter.GenresIds,
            filter.PlatformsIds,
            filter.Years,
            filter.DevelopersIds,
            filter.PublishersIds,
            filter.LocalizationIds,
            filter.Skip,
            filter.Take);

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
        return Ok(await _gamesRepository.SearchByNameAsync(name));
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

    // =====================================================================
    //  ХУК БАЗОВОГО КОНТРОЛЛЕРА
    // =====================================================================

    protected override async Task<ActionResult?> ValidateAddAsync(AddGameModel addGameModel)
    {
        Game? existingGame = await _gamesRepository.GetByNameAsync(addGameModel.Name);

        if (existingGame is not null)
            return BadRequest($"Игра с названием {addGameModel.Name} уже существует");

        return null;
    }
}