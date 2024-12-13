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
    public async Task<ActionResult<GamePlatform>> GetAll()
    {
        var gamesWithPlatforms = dataContext.GamesPlatforms.Include(g => g.Games).ToArray();
        return Ok(gamesWithPlatforms);
    }
}
