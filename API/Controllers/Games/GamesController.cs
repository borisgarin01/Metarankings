using API.Json;
using Data.Repositories.Classes.Derived.Games;
using Domain.Games;
using Domain.RequestsModels.Games;
using IdentityLibrary.Telegram;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesController : ControllerBase
{
    private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
    private readonly IMapper _mapper;
    private readonly GamesRepository _gamesRepository;
    private readonly ILogger<GamesController> _logger;
    private readonly TelegramAuthenticator _telegramAuthenticator;

    public GamesController(GamesRepository gamesRepository, IMapper mapper, TelegramAuthenticator telegramAuthenticator, ILogger<GamesController> logger)
    {
        jsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter("yyyy-MM-dd"));
        _gamesRepository = gamesRepository;
        _mapper = mapper;
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

        await _telegramAuthenticator.SendMessageAsync($"New game {addGameModel.Name} at {this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api/games/{createdGame.Id}");
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

    [HttpGet("images/uploads/{year:int}/{month:int}/{image}")]
    public async Task<IActionResult> GetImage(int year, int month, string image)
    {
        byte[]? file = await System.IO.File.ReadAllBytesAsync($"{Directory.GetCurrentDirectory()}/images/uploads/{year}/{month}/{image}");
        if (file is null)
            return NotFound();
        return File(file, "image/jpeg");
    }

    [HttpGet("genres/{genreId:long}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetGamesOfGenre(long genreId)
    {
        try
        {
            IEnumerable<Game>? gamesOfGenre = await _gamesRepository.GetByGenreIdAsync(genreId);
            if (gamesOfGenre is null)
                return NotFound();
            return Ok(gamesOfGenre);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("platforms/{platformId:long}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetGamesOfPlatform(long platformId)
    {
        try
        {
            IEnumerable<Game>? gamesOfPlatform = await _gamesRepository.GetByPlatformIdAsync(platformId);
            if (gamesOfPlatform is null)
                return NotFound();
            return Ok(gamesOfPlatform);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("developers/{developerId:long}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetGamesOfDeveloper(long developerId)
    {
        try
        {
            IEnumerable<Game>? gamesOfDeveloper = await _gamesRepository.GetByDeveloperIdAsync(developerId);
            if (gamesOfDeveloper is null)
                return NotFound();
            return Ok(gamesOfDeveloper);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("publishers/{publisherId:long}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetGamesOfPublisher(long publisherId)
    {
        try
        {
            IEnumerable<Game>? gamesOfDeveloper = await _gamesRepository.GetByPublisherIdAsync(publisherId);
            if (gamesOfDeveloper is null)
                return NotFound();
            return Ok(gamesOfDeveloper);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("year/{year:int}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetGamesOfYear(int year)
    {
        try
        {
            IEnumerable<Game>? gamesOfYear = await _gamesRepository.GetByReleaseYearAsync(year);
            if (gamesOfYear is null)
                return NotFound();
            return Ok(gamesOfYear);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}
