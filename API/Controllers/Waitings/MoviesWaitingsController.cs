using Data.Repositories.Classes.Derived.Waitings;

namespace API.Controllers.Waitings;

[Route("api/movies/[controller]")]
public sealed class MoviesWaitingsController : WaitingsControllerBase
{
    public MoviesWaitingsController(MoviesWaitingsRepository moviesWaitingsRepository, ILogger<MoviesWaitingsController> logger)
        : base(moviesWaitingsRepository, logger)
    {
    }
}
