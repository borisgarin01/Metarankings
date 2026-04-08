using API.Json;
using Data.Repositories.Classes.Derived.Games;
using Domain.Games;
using Domain.RequestsModels;
using Domain.RequestsModels.Games;
using IdentityLibrary.Telegram;

namespace API.Controllers.Games;

[ApiController]
[Route("api/games/[controller]")]
public sealed class GamesController : ControllerBase
{
    private JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
    private readonly GamesRepository _gamesRepository;
    private readonly ILogger<GamesController> _logger;
    private readonly TelegramAuthenticator _telegramAuthenticator;

    public GamesController(GamesRepository gamesRepository, TelegramAuthenticator telegramAuthenticator, ILogger<GamesController> logger)
    {
        jsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter("yyyy-MM-dd"));
        _gamesRepository = gamesRepository;
        _telegramAuthenticator = telegramAuthenticator;
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

        await _telegramAuthenticator.SendMessageAsync($"New game {addGameModel.Name} at {Request.Scheme}://{Request.Host}{Request.PathBase}/games/Details/{createdGameId}");
        return Created($"api/games/{createdGame.Id}", createdGame);
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
        Game? game = await _gamesRepository.GetAsync(id);

        if (game is null)
            return NotFound();

        return Ok(game);
    }

    [HttpPost("byParameters")]
    public async Task<ActionResult<IEnumerable<Game>>> GetByParametersAsync(GameFilterRequest filter)
    {
        IEnumerable<Game> games = await _gamesRepository.GetByParametersAsync(
            filter.GenresIds,
            filter.PlatformsIds,
            filter.Years,
            filter.DevelopersIds,
            filter.PublishersIds,
            filter.Skip,
            filter.Take
        );

        return Ok(games);
    }
}
