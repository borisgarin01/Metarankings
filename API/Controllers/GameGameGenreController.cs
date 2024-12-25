using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class GameGameGenreController : ControllerBase
{
    private readonly DataContext dataContext;

    public GameGameGenreController(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    [HttpPost]
    public async Task<ActionResult> AddGenreToGameAsync(long genreId, long gameId)
    {
        if (dataContext.GamesGamesGenres.FirstOrDefault(g => g.GameGenreId == genreId && g.GameId == gameId) is null
            && dataContext.GamesGenres.Find(genreId) is not null
            && dataContext.Games.Find(gameId) is not null)
        {
            var gameGameGenre = new GameGameGenre { GameGenreId = genreId, GameId = gameId };
            dataContext.GamesGamesGenres.Add(gameGameGenre);
            await dataContext.SaveChangesAsync();
            return Ok();
        }
        return BadRequest();
    }
}
