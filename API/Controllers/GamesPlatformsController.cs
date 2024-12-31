using AutoMapper;
using Data;
using Domain.ViewModels;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesPlatformsController : ControllerBase
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;

    public GamesPlatformsController(DataContext dataContext, IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GamePlatform>>> GetAllAsync()
    {
        return Ok(await dataContext.GamesPlatforms.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GamePlatform>> GetAsync(long id)
    {
        var gamePlatform = dataContext.GamesPlatforms.Find(id);
        if (gamePlatform is not null)
            return Ok(gamePlatform);
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync(AddGamePlatformViewModel addGamePlatformViewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var gamePlatform = mapper.Map<GamePlatform>(addGamePlatformViewModel);
                dataContext.GamesPlatforms.Add(gamePlatform);
                await dataContext.SaveChangesAsync();
                return Created($"api/GamesPlatforms/{gamePlatform.Id}", gamePlatform);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        return BadRequest(addGamePlatformViewModel);
    }
}
