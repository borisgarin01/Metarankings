using AutoMapper;
using Data;
using Domain;
using Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesAdminController : ControllerBase
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;
    public GamesAdminController(DataContext dataContext, IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult> AddGame(AddGameViewModel addGameViewModel)
    {
        if (ModelState.IsValid)
        {
            var game = mapper.Map<Game>(addGameViewModel);
            dataContext.Add(game);
            await dataContext.SaveChangesAsync();
            return Created($"api/gamesAdmin/{game.Id}", game);
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
