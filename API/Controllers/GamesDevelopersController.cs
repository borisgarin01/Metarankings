using AutoMapper;
using Data;
using Domain;
using Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesDevelopersController : ControllerBase
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;

    public GamesDevelopersController(DataContext dataContext, IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDeveloper>>> GetAllAsync()
    {
        return Ok(await dataContext.GamesDevelopers.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameDeveloper>> GetAsync(long id)
    {
        var gameDeveloper = dataContext.GamesDevelopers.Find(id);
        if (gameDeveloper is not null)
            return Ok(gameDeveloper);
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync(AddGameDeveloperViewModel addGameDeveloperViewModel)
    {
        if (ModelState.IsValid)
        {

            try
            {
                var gameDeveloper = mapper.Map<GameDeveloper>(addGameDeveloperViewModel);

                dataContext.GamesDevelopers.Add(gameDeveloper);
                await dataContext.SaveChangesAsync();
                return Created($"api/GamesDevelopers/{gameDeveloper.Id}", gameDeveloper);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        return BadRequest(addGameDeveloperViewModel);
    }
}
