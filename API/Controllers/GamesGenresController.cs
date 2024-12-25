using AutoMapper;
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
    private readonly IMapper mapper;

    public GamesGenresController(DataContext dataContext, IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
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
    public async Task<ActionResult> Add(AddGameGenreViewModel addGameGenreViewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var gameGenre = mapper.Map<GameGenre>(addGameGenreViewModel);

                dataContext.GamesGenres.Add(gameGenre);
                await dataContext.SaveChangesAsync();
                return Created($"api/GamesGenres/{gameGenre.Id}", gameGenre);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        return BadRequest(addGameGenreViewModel);
    }
}
