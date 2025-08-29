using Data.Repositories.Classes.Derived.Games;
using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.Reviews;
using IdentityLibrary.DTOs;
using IdentityLibrary.Telegram;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesGamersReviewsController : ControllerBase
{
    private readonly GamesPlayersReviewsRepository _gamesPlayersReviewsRepository;
    private readonly GamesRepository _gamesRepository;

    private readonly UserManager<ApplicationUser> _usersManager;

    private readonly TelegramAuthenticator _telegramAuthenticator;

    private readonly ILogger<GamesGamersReviewsController> _logger;

    public GamesGamersReviewsController(GamesPlayersReviewsRepository gamesPlayersReviewsRepository, TelegramAuthenticator telegramAuthenticator, GamesRepository gamesRepository, UserManager<ApplicationUser> usersManager, ILogger<GamesGamersReviewsController> logger)
    {
        _gamesPlayersReviewsRepository = gamesPlayersReviewsRepository;
        _telegramAuthenticator = telegramAuthenticator;
        _gamesRepository = gamesRepository;
        _usersManager = usersManager;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> AddGameReviewAsync(AddGamePlayerReviewModel addGameReviewModel)
    {
        long userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        GameReview gamePlayerReviewToCheckExistance = await _gamesPlayersReviewsRepository.GetUserReviewForGameAsync(userId, addGameReviewModel.GameId);

        if (gamePlayerReviewToCheckExistance is not null)
            return BadRequest($"У пользователя {userId} уже есть отзыв на игру {addGameReviewModel.GameId}");

        Domain.Games.Game game = await _gamesRepository.GetAsync(addGameReviewModel.GameId);
        if (game is null)
            return NotFound("Game not found");

        var addGameReviewWithUserIdAndDateModel = new AddGamePlayerReviewWithUserIdAndDateModel(addGameReviewModel.GameId, addGameReviewModel.TextContent, addGameReviewModel.Score, long.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value), DateTime.Now);

        var gameReviewId = await _gamesPlayersReviewsRepository.AddAsync(addGameReviewWithUserIdAndDateModel);
        var createdGameReview = await _gamesPlayersReviewsRepository.GetAsync(gameReviewId);
        await _telegramAuthenticator.SendMessageAsync($"New game review for game {game.Name} at {this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api/GamesGamersReviews/{createdGameReview.Id}");
        return Created($"api/GamesReviews/{createdGameReview.Id}", createdGameReview);

    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<GameReview>> GetReview(long id)
    {
        GameReview gameReview = await _gamesPlayersReviewsRepository.GetAsync(id);
        if (gameReview is null)
            return NotFound();
        return Ok(gameReview);
    }

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<GameReview>> UpdateReview(long id, UpdateGamePlayerReviewModel updateGamePlayerReviewModel)
    {
        GameReview gameReview = await _gamesPlayersReviewsRepository.GetAsync(id);
        if (gameReview is null)
            return NotFound();

        if (long.Parse(User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value) != gameReview.UserId)
            return BadRequest("User are not a review author");

        else
            try
            {
                GameReview updatedGameReview = await _gamesPlayersReviewsRepository.UpdateAsync(updateGamePlayerReviewModel, id);
                return Ok(updatedGameReview);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return StatusCode(500, $"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<GameReview>> RemoveReview(long id)
    {
        GameReview gameReview = await _gamesPlayersReviewsRepository.GetAsync(id);
        if (gameReview is null)
            return NotFound();

        if ((long.Parse(User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value) != gameReview.UserId)
            && User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role && a.Value == "Admin") is null)
            return BadRequest("User are not a review author");

        try
        {
            await _gamesPlayersReviewsRepository.RemoveAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return StatusCode(500, $"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
    }
}
