using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MainPageController : ControllerBase
{
    private readonly DataContext dataContext;

    public MainPageController(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    [HttpGet]
    public async Task<ActionResult<Game>> GetAll()
    {
        var gamesWithPlatforms = dataContext.Games.Include(g => g.Platforms).ToArray();
        return Ok(gamesWithPlatforms);
    }
}
