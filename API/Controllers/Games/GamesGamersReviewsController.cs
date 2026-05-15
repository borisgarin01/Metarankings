using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.Reviews;
using IdentityLibrary.DTOs;
using IdentityLibrary.Telegram;

namespace API.Controllers.Games;

[ApiController]
[Route("api/games/[controller]")]
public sealed class GamesGamersReviewsController : ControllerBase
{
    private readonly IGamesPlayersReviewsRepository _gamesPlayersReviewsRepository;
    private readonly IGamesRepository _gamesRepository;
    private readonly GamesPlayersReviewsShiftsRepository _gamePlayerReviewsShiftsRepository;

    private readonly UserManager<ApplicationUser> _usersManager;

    private readonly TelegramAuthenticator _telegramAuthenticator;

    private readonly ILogger<GamesGamersReviewsController> _logger;

    public GamesGamersReviewsController(IGamesPlayersReviewsRepository gamesPlayersReviewsRepository, TelegramAuthenticator telegramAuthenticator, IGamesRepository gamesRepository, UserManager<ApplicationUser> usersManager, ILogger<GamesGamersReviewsController> logger, GamesPlayersReviewsShiftsRepository gamePlayerReviewsShiftsRepository)
    {
        _gamesPlayersReviewsRepository = gamesPlayersReviewsRepository;
        _telegramAuthenticator = telegramAuthenticator;
        _gamesRepository = gamesRepository;
        _usersManager = usersManager;
        _logger = logger;
        _gamePlayerReviewsShiftsRepository = gamePlayerReviewsShiftsRepository;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> AddGameReviewAsync(AddGamePlayerReviewModel addGameReviewModel)
    {
        long userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        GameReview gamePlayerReviewToCheckExistance = await _gamesPlayersReviewsRepository.GetUserReviewForGameAsync(userId, addGameReviewModel.GameId);

        if (gamePlayerReviewToCheckExistance is not null)
            return BadRequest($"У пользователя {userId} уже есть отзыв на игру {addGameReviewModel.GameId}");

        Domain.Games.Game game = await _gamesRepository.GetAsync(addGameReviewModel.GameId);
        if (game is null)
            return NotFound("Game not found");

        AddGamePlayerReviewWithUserIdAndDateModel addGameReviewWithUserIdAndDateModel = new(addGameReviewModel.GameId, addGameReviewModel.TextContent, addGameReviewModel.Score, long.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value), DateTime.Now);

        long gameReviewId = await _gamesPlayersReviewsRepository.AddAsync(addGameReviewWithUserIdAndDateModel);
        GameReview createdGameReview = await _gamesPlayersReviewsRepository.GetAsync(gameReviewId);
        await _telegramAuthenticator.SendMessageAsync($"New game review for game {game.Name} at {Request.Scheme}://{Request.Host}{Request.PathBase}/games/details/{createdGameReview.GameId}");
        return Created($"api/GamesReviews/{createdGameReview.Id}", createdGameReview);

    }

    [HttpGet("{offset:long}/{limit:long}")]
    public async Task<ActionResult<IEnumerable<GameReview>>> GetReviewsAsync(long offset, long limit)
    {
        IEnumerable<GameReview> gamesReviews = await _gamesPlayersReviewsRepository.GetAsync(offset, limit);
        return Ok(gamesReviews);
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    [HttpPost("shift")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<long>> Shift(Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Frontend.AddGamePlayerReviewShiftModel addGamePlayerReviewShiftModel)
    {

        try
        {
            _logger.LogInformation("ShifterId - {ShifterId}", User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value);

            long shifterId = long.Parse(User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value);

            _logger.LogInformation("GamePlayerReviewId - {GamePlayerReviewId}, Direction - {Direction}, ShifterId - {ShifterId}", addGamePlayerReviewShiftModel.GamePlayerReviewId, addGamePlayerReviewShiftModel.Direction, shifterId);

            GameReview gameReview = await _gamesPlayersReviewsRepository.GetAsync(addGamePlayerReviewShiftModel.GamePlayerReviewId);

            _logger.LogInformation("Game review: Id - {Id}, GameId - {GameId}", gameReview.Id, gameReview.GameId);

            if (gameReview is null)
            {
                _logger.LogWarning("gameReview is null");
                return BadRequest("gameReview is null");
            }
            if (shifterId == gameReview.UserId)
            {
                _logger.LogWarning("shifterId == gameReview.UserId. Нельзя голосовать за свои обзоры");
                return BadRequest("Нельзя голосовать за свои обзоры");
            }
            GamePlayerReviewShift shift = await _gamePlayerReviewsShiftsRepository.GetByShifterIdAsync(long.Parse(User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value), gameReview.Id);
            if (shift is null)
            {
                long insertedShift = await _gamePlayerReviewsShiftsRepository.AddAsync(new Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend.AddGamePlayerReviewShiftModel(gameReview.Id, shifterId, addGamePlayerReviewShiftModel.Direction));
                return Ok(insertedShift);
            }
            else if (shift.Direction != addGamePlayerReviewShiftModel.Direction)
            {
                GamePlayerReviewShift gamePlayerReviewShift = await _gamePlayerReviewsShiftsRepository.UpdateAsync(new Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend.UpdateGamePlayerReviewShiftModel(shift.GamePlayerReviewId, shift.ShifterId, addGamePlayerReviewShiftModel.Direction), shift.Id);
                return Ok(gamePlayerReviewShift);
            }
            _logger.LogWarning("Пользователь уже голосовал за обзор");
            return BadRequest("Пользователь уже голосовал за обзор");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.StackTrace);
            return StatusCode(500, ex);
        }
    }
}
