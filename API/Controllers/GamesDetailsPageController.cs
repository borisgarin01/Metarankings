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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> GetAllGamesAsync()
    {
        var games = await dataContext.Games.Include(g => g.Genres).Include(g => g.CriticsReviews).Include(g => g.Developers).Include(g => g.Platforms).Include(g => g.Tags).Include(g => g.UsersReviews).ToArrayAsync();
        return Ok(games);
    }
}
