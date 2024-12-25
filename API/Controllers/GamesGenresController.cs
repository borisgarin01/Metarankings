using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesGenresController : ControllerBase
{
    private readonly DataContext dataContext;

    public GamesGenresController(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameGenre>>> GetAllAsync()
    {
        return Ok(await dataContext.GamesGenres.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameGenre>> GetAsync(long id)
    {
        var gameGenre = dataContext.GamesGenres.Find(id);
        if (gameGenre is not null)
            return Ok(gameGenre);
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult> Add(GameGenre gameGenre)
    {
        if (ModelState.IsValid)
        {
            try
            {
                dataContext.GamesGenres.Add(gameGenre);
                await dataContext.SaveChangesAsync();
                return Created($"api/GamesGenres/{gameGenre.Id}", gameGenre);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        return BadRequest(gameGenre);
    }
}
