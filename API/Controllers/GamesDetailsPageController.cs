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
    public async Task<ActionResult<IEnumerable<Game>>> GetAllGamesAsync(int pageSize, int page)
    {
        var games = await dataContext.Games
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(g => g.Genres)
            .Include(g => g.CriticsReviews)
            .Include(g => g.Developers)
            .Include(g => g.Platforms)
            .Include(g => g.Localization)
            .Include(g => g.Tags)
            .Include(g => g.Publishers)
            .Include(g => g.UsersReviews)
            .Include(g => g.Trailers).ToArrayAsync();
        return Ok(games);
    }
}
