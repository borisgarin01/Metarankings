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
    private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
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

    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetAsync(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> games = await _gamesRepository.GetAsync((pageNumber - 1) * pageSize, pageSize);
        return Ok(games);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<long>> AddAsync(AddGameModel addGameModel)
    {
        long createdGameId = await _gamesRepository.AddAsync(addGameModel);

        Game createdGame = await _gamesRepository.GetAsync(createdGameId);

        await _telegramAuthenticator.SendMessageAsync($"New game {addGameModel.Name} at {this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/games/Details/{createdGameId}");
        return Created($"api/games/{createdGame.Id}", createdGame);
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
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
        var games = await _gamesRepository.GetAllAsync();
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
