using Data.Repositories.Classes.Derived.Waitings;

namespace API.Controllers.Waitings;

[Route("api/games/[controller]")]
public sealed class GamesWaitingsController : WaitingsControllerBase
{
    public GamesWaitingsController(GamesWaitingsRepository gamesWaitingsRepository, ILogger<GamesWaitingsController> logger)
        : base(gamesWaitingsRepository, logger)
    {
    }
}
