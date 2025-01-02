using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesDetailsPageController : ControllerBase
{
    private readonly DataContext dataContext;

    public GamesDetailsPageController(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    [HttpGet("pageSize={pageSize}&page={page}")]
    public async Task<ActionResult<Game[]>> GetAllGamesAsync(int pageSize, int page)
    {
        var games = await dataContext.Games
            .Include(g => g.Developers) // Include Developers
            .Include(g => g.Publishers) // Include Publishers
            .Include(g => g.Genres) // Include Genres
            .Include(g => g.Platforms)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToArrayAsync();

        return Ok(games);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<Game>> GetGameAsync(string name)
    {
        var game = await dataContext.Games.Include(g => g.Genres)
            .Include(g => g.CriticsReviews)
            .Include(g => g.Developers)
            .Include(g => g.Platforms)
            .Include(g => g.Localization)
            .Include(g => g.Tags)
            .Include(g => g.Publishers)
            .Include(g => g.UsersReviews)
            .Include(g => g.Trailers).
            FirstOrDefaultAsync(g => g.Name == name);

        if (game is not null)
        {
            return Ok(game);
        }

        return NotFound();
    }
}
