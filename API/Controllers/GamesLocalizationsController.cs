using AutoMapper;
using Data;
using Domain.ViewModels;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesLocalizationsController : ControllerBase
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;

    public GamesLocalizationsController(DataContext dataContext, IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameLocalization>>> GetAll()
    {
        var gamesLocalizations = await dataContext.GamesLocalizations.AsNoTracking().ToArrayAsync();
        return Ok(gamesLocalizations);
    }

    [HttpPost]
    public async Task<ActionResult> AddGameLocalization(GameLocalizationViewModel gameLocalizationViewModel)
    {
        if (ModelState.IsValid)
        {
            var gameLocalization = mapper.Map<GameLocalization>(gameLocalizationViewModel);
            dataContext.Add(gameLocalization);
            await dataContext.SaveChangesAsync();
            return Created($"api/gamesAdmin/{gameLocalization.Id}", gameLocalization);
        }
        return BadRequest(gameLocalizationViewModel);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameLocalization>> GetGameLocalization(long id)
    {
        var gameLocalization = await dataContext.GamesLocalizations.FindAsync(id);
        if (gameLocalization is not null)
        {
            return Ok(gameLocalization);
        }
        return NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveGameLocalization(long id)
    {
        var gameLocalization = await dataContext.GamesLocalizations
            .FindAsync(id);

        if (gameLocalization is null)
            return NotFound();

        dataContext.Remove(gameLocalization);
        await dataContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch]
    public async Task<ActionResult<GameLocalization>> UpdateGameLocalization(long id, GameLocalizationViewModel gameLocalizationViewModel)
    {
        var gameLocalization = await dataContext.GamesLocalizations.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

        if (gameLocalization is null)
            return NotFound();

        try
        {
            gameLocalization = mapper.Map<GameLocalization>(gameLocalizationViewModel);
            gameLocalization.Id = id;
            try
            {
                dataContext.Update(gameLocalization);
                await dataContext.SaveChangesAsync();
                return Ok(gameLocalization);
            }
            catch
            {
                return StatusCode(500, gameLocalizationViewModel);
            }
        }
        catch
        {
            return BadRequest(gameLocalizationViewModel);
        }
    }
}
