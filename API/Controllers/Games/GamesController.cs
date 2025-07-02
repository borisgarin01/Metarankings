using API.Json;
using API.Models.RequestsModels.Games;
using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Interfaces;
using Data.Repositories.Interfaces.Derived;
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
    private readonly PublishersRepository _publishersRepository;
    private readonly PlatformsRepository _platformsRepository;
    private readonly ILocalizationsRepository _localizationsRepository;

    public GamesController(GamesRepository gamesRepository, ILocalizationsRepository localizationsRepository, PublishersRepository publishersRepository, PlatformsRepository platformsRepository)
    {
        jsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter("yyyy-MM-dd"));
        _gamesRepository = gamesRepository;
        _localizationsRepository = localizationsRepository;
        _publishersRepository = publishersRepository;
        _platformsRepository = platformsRepository;
    }

    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetAsync(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
    {
        var games = await _gamesRepository.GetAsync((pageNumber - 1) * pageSize, pageSize);
        return Ok(games);
    }

    [HttpPost]
    public async Task<ActionResult<long>> AddAsync(GameModel gameModel)
    {
        var localization = await _localizationsRepository.GetByNameAsync(gameModel.Localization.Name);

        long localizationId = 0;
        if (localization is null)
        {
            var localizationToAdd = new Localization { Name = localization.Name };

            localizationId = await _localizationsRepository.AddAsync(localizationToAdd);
        }
        else
        {
            localizationId = localization.Id;
        }

        var publiher = await _publishersRepository.GetByNameAsync(gameModel.Publisher.Name);

        long publisherId = 0;
        if (publiher is null)
        {
            var publisherToAdd = new Publisher { Name = gameModel.Publisher.Name };

            publisherId = await _publishersRepository.AddAsync(publisherToAdd);
        }
        else
        {
            publisherId = publiher.Id;
        }

        var platformsToAddToGame = new List<Platform>();

        foreach (var platform in gameModel.Platforms)
        {
            long platformId = 0;
            Platform platformToCheckExistance = await _platformsRepository.GetByNameAsync(platform.Name);
            if (platformToCheckExistance is null)
            {
                var platformToAdd = new Platform { Name = platformToCheckExistance.Name };
                platformId = await _platformsRepository.AddAsync(platformToAdd);
                platformToAdd.Id = platformId;
                platformsToAddToGame.Add(platformToAdd);
            }
            else
            {
                platformsToAddToGame.Add(platformToCheckExistance);
            }
        }

        var game = new Game
        {
            Description = gameModel.Description,
            Developers = gameModel.Developers.Select(d => new Developer { Name = d.Name }).ToList(),
            Genres = gameModel.Genres.Select(g => new Genre { Name = g.Name }).ToList(),
            Image = gameModel.Image,
            Localization = new Localization
            {
                Id = localizationId,
                Name = localization.Name
            },
            Name = gameModel.Name,
            Platforms = platformsToAddToGame,
            Publisher = publiher,
            PublisherId = publisherId,
            ReleaseDate = gameModel.ReleaseDate,
            Screenshots = new List<GameScreenshot>(),
            Tags = new List<Tag>(),
            Trailer = null
        };

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

    [HttpGet("platforms/{platformId}")]
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

    [HttpGet("developers/{developerId}")]
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

    [HttpGet("publishers/{publisherId}")]
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
