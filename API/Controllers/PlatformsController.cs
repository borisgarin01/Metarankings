using Data.Repositories.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesPlatformsController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IGamesPlatformsRepository gamesPlatformsRepository;

    public GamesPlatformsController(IConfiguration configuration, IGamesPlatformsRepository gamesPlatformsRepository)
    {
        this.configuration = configuration;
        this.gamesPlatformsRepository = gamesPlatformsRepository;
    }

    [HttpPost]
    public async Task<ActionResult> AddGameAsync(GamePlatform gamePlatform)
    {
        await gamesPlatformsRepository.AddAsync(gamePlatform);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GamePlatform>>> GetGamesPlatformsAsync()
    {
        var moviesGenres = await gamesPlatformsRepository.GetAllAsync();
        return Ok(moviesGenres);
    }
}
