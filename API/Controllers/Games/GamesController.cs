using API.Json;
using API.Models.RequestsModels.Games;
using Data.Repositories.Classes.Derived.Games;
using Domain.Games;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesController : ControllerBase
{
    private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
    private readonly IMapper _mapper;
    private readonly GamesRepository _gamesRepository;

    public GamesController(GamesRepository gamesRepository, IMapper mapper)
    {
        jsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter("yyyy-MM-dd"));
        _gamesRepository = gamesRepository;
        _mapper = mapper;
    }

    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetAsync(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
    {
        var games = await _gamesRepository.GetAsync((pageNumber - 1) * pageSize, pageSize);
        return Ok(games);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<long>> AddAsync(AddGameModel gameModel)
    {
        var game = _mapper.Map<Game>(gameModel);
        var createdGame = await _gamesRepository.AddAsync(game);
        return Ok(createdGame);
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
        var game = await _gamesRepository.GetAsync(id);

        if (game is null)
            return NotFound();

        return Ok(game);
    }

    [HttpGet("images/uploads/{year:int}/{month:int}/{image}")]
    public async Task<IActionResult> GetImage(int year, int month, string image)
    {
        var file = await System.IO.File.ReadAllBytesAsync($"{Directory.GetCurrentDirectory()}/images/uploads/{year}/{(month < 10 ? $"0{month}" : $"{month}")}/{image}");
        if (file is null)
            return NotFound();
        return File(file, "image/jpeg");
    }

    [HttpGet("genres/{genreId:long}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetGamesOfGenre(long genreId)
    {
        try
        {
            var gamesOfGenre = await _gamesRepository.GetByGenreIdAsync(genreId);
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
            var gamesOfPlatform = await _gamesRepository.GetByPlatformIdAsync(platformId);
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
            var gamesOfDeveloper = await _gamesRepository.GetByDeveloperIdAsync(developerId);
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
            var gamesOfDeveloper = await _gamesRepository.GetByPublisherIdAsync(publisherId);
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
            var gamesOfYear = await _gamesRepository.GetByReleaseYearAsync(year);
            if (gamesOfYear is null)
                return NotFound();
            return Ok(gamesOfYear);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPost("gamesByParameters")]
    public async Task<ActionResult<IEnumerable<Game>>> GetGamesByParameters(GamesGettingRequestModel gamesGettingRequestModel)
    {
        try
        {
            var games = await _gamesRepository.GetByParametersAsync(gamesGettingRequestModel);
            if (games is null)
                return NotFound();
            return Ok(games);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}
