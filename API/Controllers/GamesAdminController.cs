using AutoMapper;
using Data;
using Domain;
using Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesAdminController : ControllerBase
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;
    public GamesAdminController(DataContext dataContext, IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        // Example: Save file to the server (adjust the path as needed)
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return the file path or URL to the caller (adjust as needed)
        var fileUrl = $"api/GamesImages/{file.FileName}";
        return Ok(fileUrl);
    }

    [HttpPost]
    public async Task<ActionResult> AddGame(AddGameViewModel addGameViewModel)
     {
        if (ModelState.IsValid)
        {
            var game = new Game
            {
                Description = addGameViewModel.Description,
                DetailsImageSource = addGameViewModel.DetailsImageSource,
                ListImageSource = addGameViewModel.ListImageSource,
                LocalizationId = addGameViewModel.LocalizationId.Value,
                Name = addGameViewModel.Name,
                ReleaseDate = addGameViewModel.ReleaseDate.Value,
                Score = addGameViewModel.Score
            };
            dataContext.Add(game);
            dataContext.SaveChanges();

            foreach (var genre in addGameViewModel.GameGenres)
            {
                var gameGameGenre = new GameGameGenre { GameId = game.Id, GameGenreId = genre.Id };
                dataContext.Add(gameGameGenre);
            }

            foreach (var platform in addGameViewModel.Platforms)
            {
                var gameGamePlatform = new GameGamePlatform { GamePlatformId = platform.Id, GameId = game.Id };
                dataContext.Add(gameGamePlatform);
            }

            foreach (var developer in addGameViewModel.Developers)
            {
                var gameGameDeveloper = new GameGameDeveloper { GameDeveloperId = developer.Id, GameId = game.Id };
                dataContext.Add(gameGameDeveloper);
            }

            foreach (var publisher in addGameViewModel.Publishers)
            {
                var gameGamePublisher = new GameGamePublisher { GamePublisherId = publisher.Id, GameId = game.Id };
                dataContext.Add(gameGamePublisher);
            }
            await dataContext.SaveChangesAsync();

            var createdGame = await dataContext.Games.Include(a => a.Genres).Include(a => a.Platforms).Include(a => a.Developers).Include(a => a.Publishers).FirstOrDefaultAsync(a => a.Id == game.Id);
            return Created($"api/gamesAdmin/{game.Id}", createdGame);
        }
        return BadRequest(addGameViewModel);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Game>> GetGame(long id)
    {
        var game = await dataContext.Games.FindAsync(id);
        if (game is not null)
        {
            return Ok(game);
        }
        return NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveGame(long id)
    {
        var game = await dataContext.Games.FindAsync(id);

        if (game is null)
            return NotFound();

        dataContext.Remove(game);
        await dataContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch]
    public async Task<ActionResult> UpdateGame(long id, UpdateGameViewModel updateGameViewModel)
    {
        var game = await dataContext.Games.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

        if (game is null)
            return NotFound();

        try
        {
            game = mapper.Map<Game>(updateGameViewModel);
            game.Id = id;
            try
            {
                dataContext.Update(game);
                await dataContext.SaveChangesAsync();
                return Ok(game);
            }
            catch
            {
                return StatusCode(500, updateGameViewModel);
            }
        }
        catch
        {
            return BadRequest(updateGameViewModel);
        }
    }
}
