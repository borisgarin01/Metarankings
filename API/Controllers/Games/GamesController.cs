using API.Json;
using Data.Repositories.Classes.Derived.Games;
using Domain.Games;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesController : ControllerBase
{
    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    private readonly GamesRepository _gamesRepository;

    public GamesController(GamesRepository gamesRepository)
    {
        jsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter("yyyy-MM-dd"));
        _gamesRepository = gamesRepository;
    }

    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    public async Task<ActionResult<IEnumerable<GameModel>>> GetAsync(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
    {
        var games = await _gamesRepository.GetAsync((pageNumber - 1) * pageSize, pageSize);
        return Ok(games);
    }

    [HttpPost]
    public async Task<ActionResult<long>> AddAsync(GameModel gameModel)
    {
        var createdGame = await _gamesRepository.AddAsync(gameModel);
        return Ok(createdGame);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameModel>>> GetAsync(CancellationToken cancellationToken = default)
    {
        var games = await _gamesRepository.GetAllAsync();
        return Ok(games);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<GameModel>> GetAsync(long id)
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

    [HttpGet("genres/{genreUrlPart}")]
    public async Task<ActionResult<IEnumerable<GameModel>>> GetGamesOfGenre(string genreUrlPart)
    {
        try
        {
            var genreUrl = $"/genres/{genreUrlPart}";
            var gamesOfGenre = await _gamesRepository.GetByGenreUrlAsync(genreUrl);
            if (gamesOfGenre is null)
                return NotFound();
            return Ok(gamesOfGenre);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("platforms/{platformUrlPart}")]
    public async Task<ActionResult<IEnumerable<GameModel>>> GetGamesOfPlatform(string platformUrlPart)
    {
        try
        {
            var platformUrl = $"/platforms/{platformUrlPart}";
            var gamesOfPlatform = await _gamesRepository.GetByPlatformUrlAsync(platformUrl);
            if (gamesOfPlatform is null)
                return NotFound();
            return Ok(gamesOfPlatform);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("developers/{developerUrlPart}")]
    public async Task<ActionResult<IEnumerable<GameModel>>> GetGamesOfDeveloper(string developerUrlPart)
    {
        try
        {
            var developerUrl = $"/developers/{developerUrlPart}";
            var gamesOfDeveloper = await _gamesRepository.GetByDeveloperUrlAsync(developerUrl);
            if (gamesOfDeveloper is null)
                return NotFound();
            return Ok(gamesOfDeveloper);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("publishers/{publisherUrlPart}")]
    public async Task<ActionResult<IEnumerable<GameModel>>> GetGamesOfPublisher(string publisherUrlPart)
    {
        try
        {
            var developerUrl = $"/publishers/{publisherUrlPart}";
            var gamesOfDeveloper = await _gamesRepository.GetByPublisherUrlAsync(developerUrl);
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
    public async Task<ActionResult<IEnumerable<GameModel>>> GetGamesOfYear(int year)
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
    public async Task<ActionResult<IEnumerable<GameModel>>> GetGamesByParameters(GamesGettingRequestModel gamesGettingRequestModel)
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
