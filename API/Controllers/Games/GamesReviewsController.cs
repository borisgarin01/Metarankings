using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.Reviews;
using IdentityLibrary.Telegram;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesReviewsController : ControllerBase
{
    private readonly IGamesReviewsRepository _gamesReviewsRepository;
    private readonly GamesRepository _gamesRepository;

    private readonly TelegramAuthenticator _telegramAuthenticator;

    public GamesReviewsController(IGamesReviewsRepository gamesReviewsRepository, TelegramAuthenticator telegramAuthenticator, GamesRepository gamesRepository)
    {
        _gamesReviewsRepository = gamesReviewsRepository;
        _telegramAuthenticator = telegramAuthenticator;
        _gamesRepository = gamesRepository;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> AddGameReviewAsync(GameReview gameReview)
    {
        Domain.Games.Game game = await _gamesRepository.GetAsync(gameReview.Id);
        if (game is null)
            return NotFound("Game not found");
        var gameReviewId = await _gamesReviewsRepository.AddAsync(gameReview);
        var createdGameReview = await _gamesReviewsRepository.GetAsync(gameReviewId);
        await _telegramAuthenticator.SendMessageAsync($"New game review for {game.Name}  at {this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api/gamesreviews/{gameReview.Id}");
        return Created($"api/gamesreviews/{gameReview.Id}", createdGameReview);

    }
}
