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
#if DEBUG
        if (this.dataContext.Games.Count() == 0)
        {
            FetchData();
        }
#endif
    }

    private void FetchData()
    {
        var gamesDevelopers = Enumerable.Range(0, 100).Select(a => new GameDeveloper { Name = $"Developer {a + 1}", Url = $"developer{a + 1}" });
        var critics = Enumerable.Range(0, 100).Select(a => new Critic { Name = $"Critic {a + 1}", Url = $"critic{a + 1}" });
        var gamesGenres = Enumerable.Range(0, 100).Select(a => new GameGenre { Name = $"Genre {a + 1}", Url = $"genre{a + 1}" });
        var gamesLocalizations = Enumerable.Range(0, 100).Select(a => new GameLocalization { Title = $"GameLocalization {a + 1}", Url = $"gameLocalization{a + 1}" });
        var gamesPlatforms = Enumerable.Range(0, 100).Select(a => new GamePlatform { Name = $"GameLocalization {a + 1}", Url = $"gamePlatform{a + 1}" });
        var gamesPublishers = Enumerable.Range(0, 100).Select(a => new GamePublisher { Name = $"GamePublisher {a + 1}", Url = $"gamePublisher{a + 1}" });
        var gamers = Enumerable.Range(0, 100).Select(a => new Gamer { AccountName = $"Gamer {a + 1}", Url = $"gamer{a + 1}" });
        var gamesTags = Enumerable.Range(0, 100).Select(a => new GameTag { Title = $"GameTag {a + 1}", Url = $"gameTag{a + 1}" });

        dataContext.GamesDevelopers.AddRange(gamesDevelopers);
        dataContext.Critics.AddRange(critics);
        dataContext.GamesGenres.AddRange(gamesGenres);
        dataContext.GamesLocalizations.AddRange(gamesLocalizations);
        dataContext.GamesPlatforms.AddRange(gamesPlatforms);
        dataContext.GamesPublishers.AddRange(gamesPublishers);
        dataContext.Gamers.AddRange(gamers);
        dataContext.GamesTags.AddRange(gamesTags);

        dataContext.SaveChanges();
    }

    [HttpGet]
    public async Task<ActionResult<GamePlatform>> GetAll()
    {
        var gamesWithPlatforms = dataContext.GamesPlatforms.Include(g => g.Games).ToArray();
        return Ok(gamesWithPlatforms);
    }
}
