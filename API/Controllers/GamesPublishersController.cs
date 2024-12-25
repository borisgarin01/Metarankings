using AutoMapper;
using Data;
using Domain.ViewModels;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesPublishersController : ControllerBase
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;

    public GamesPublishersController(DataContext dataContext, IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GamePublisher>>> GetAllAsync()
    {
        return Ok(await dataContext.GamesPublishers.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GamePublisher>> GetAsync(long id)
    {
        var gamePublisher = dataContext.GamesPublishers.Find(id);
        if (gamePublisher is not null)
            return Ok(gamePublisher);
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync(AddGamePublisherViewModel addGamePublisherViewModel)
    {
        if (ModelState.IsValid)
        {

            try
            {
                var gamePublisher = mapper.Map<GamePublisher>(addGamePublisherViewModel);

                dataContext.GamesPublishers.Add(gamePublisher);
                await dataContext.SaveChangesAsync();
                return Created($"api/GamesPublishers/{gamePublisher.Id}", gamePublisher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        return BadRequest(addGamePublisherViewModel);
    }
}
