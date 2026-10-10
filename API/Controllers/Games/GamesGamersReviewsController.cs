using API.Auth;
using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.Reviews;
using FrontendShift = Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Frontend;
using BackendShift = Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend;

namespace API.Controllers.Games;

[ApiController]
[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Games })]
[Route("api/games/[controller]")]
public sealed class GamesGamersReviewsController : ControllerBase
{
    private readonly IGamesPlayersReviewsRepository _gamesPlayersReviewsRepository;
    private readonly IGamesRepository _gamesRepository;
    private readonly GamesPlayersReviewsShiftsRepository _gamePlayerReviewsShiftsRepository;

    private readonly ILogger<GamesGamersReviewsController> _logger;

    public GamesGamersReviewsController(IGamesPlayersReviewsRepository gamesPlayersReviewsRepository, IGamesRepository gamesRepository, ILogger<GamesGamersReviewsController> logger, GamesPlayersReviewsShiftsRepository gamePlayerReviewsShiftsRepository)
    {
        _gamesPlayersReviewsRepository = gamesPlayersReviewsRepository;
        _gamesRepository = gamesRepository;
        _logger = logger;
        _gamePlayerReviewsShiftsRepository = gamePlayerReviewsShiftsRepository;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> AddGameReviewAsync(AddGamePlayerReviewModel addGameReviewModel)
    {
        if (User.GetUserId() is not long userId)
            return Unauthorized();

        GameReview? existingReview = await _gamesPlayersReviewsRepository.GetUserReviewForGameAsync(userId, addGameReviewModel.GameId);
        if (existingReview is not null)
            return BadRequest($"У пользователя {userId} уже есть отзыв на игру {addGameReviewModel.GameId}");

        Game? game = await _gamesRepository.GetAsync(addGameReviewModel.GameId);
        if (game is null)
            return NotFound("Game not found");

        if (!game.IsReleased)
            return BadRequest("Оценки и отзывы можно оставлять только после выхода игры");

        AddGamePlayerReviewWithUserIdAndDateModel addGameReviewWithUserIdAndDateModel = new(addGameReviewModel.GameId, addGameReviewModel.TextContent, addGameReviewModel.Score, userId, DateTime.Now);

        long gameReviewId = await _gamesPlayersReviewsRepository.AddAsync(addGameReviewWithUserIdAndDateModel);
        GameReview createdGameReview = await _gamesPlayersReviewsRepository.GetAsync(gameReviewId);
        return Created($"api/GamesReviews/{createdGameReview.Id}", createdGameReview);
    }

    [HttpGet("{offset:long}/{limit:long}")]
    public async Task<ActionResult<IEnumerable<GameReview>>> GetReviewsAsync(long offset, long limit)
    {
        return Ok(await _gamesPlayersReviewsRepository.GetAsync(offset, limit));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<GameReview>> GetReview(long id)
    {
        GameReview? gameReview = await _gamesPlayersReviewsRepository.GetAsync(id);
        if (gameReview is null)
            return NotFound();

        return Ok(gameReview);
    }

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<GameReview>> UpdateReview(long id, UpdateGamePlayerReviewModel updateGamePlayerReviewModel)
    {
        GameReview? gameReview = await _gamesPlayersReviewsRepository.GetAsync(id);
        if (gameReview is null)
            return NotFound();

        if (User.GetUserId() != gameReview.UserId)
            return BadRequest("User are not a review author");

        Game? game = await _gamesRepository.GetAsync(gameReview.GameId);
        if (game is null || !game.IsReleased)
            return BadRequest("Оценки и отзывы можно оставлять только после выхода игры");

        return Ok(await _gamesPlayersReviewsRepository.UpdateAsync(updateGamePlayerReviewModel, id));
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<GameReview>> RemoveReview(long id)
    {
        GameReview? gameReview = await _gamesPlayersReviewsRepository.GetAsync(id);
        if (gameReview is null)
            return NotFound();

        if (User.GetUserId() != gameReview.UserId && !User.HasClaim(ClaimTypes.Role, "Admin"))
            return BadRequest("User are not a review author");

        await _gamesPlayersReviewsRepository.RemoveAsync(id);
        return NoContent();
    }

    [HttpPost("shift")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<long>> Shift(FrontendShift.AddGamePlayerReviewShiftModel addGamePlayerReviewShiftModel)
    {
        if (User.GetUserId() is not long shifterId)
            return Unauthorized();

        _logger.LogInformation("GamePlayerReviewId - {GamePlayerReviewId}, Direction - {Direction}, ShifterId - {ShifterId}", addGamePlayerReviewShiftModel.GamePlayerReviewId, addGamePlayerReviewShiftModel.Direction, shifterId);

        GameReview? gameReview = await _gamesPlayersReviewsRepository.GetAsync(addGamePlayerReviewShiftModel.GamePlayerReviewId);
        if (gameReview is null)
            return NotFound("Отзыв не найден");

        if (shifterId == gameReview.UserId)
            return BadRequest("Нельзя голосовать за свои обзоры");

        GamePlayerReviewShift? shift = await _gamePlayerReviewsShiftsRepository.GetByShifterIdAsync(shifterId, gameReview.Id);
        if (shift is null)
        {
            long insertedShiftId = await _gamePlayerReviewsShiftsRepository.AddAsync(new BackendShift.AddGamePlayerReviewShiftModel(gameReview.Id, shifterId, addGamePlayerReviewShiftModel.Direction));
            return Ok(insertedShiftId);
        }

        if (shift.Direction != addGamePlayerReviewShiftModel.Direction)
        {
            GamePlayerReviewShift updatedShift = await _gamePlayerReviewsShiftsRepository.UpdateAsync(new BackendShift.UpdateGamePlayerReviewShiftModel(shift.GamePlayerReviewId, shift.ShifterId, addGamePlayerReviewShiftModel.Direction), shift.Id);
            return Ok(updatedShift.Id);
        }

        return BadRequest("Пользователь уже голосовал за обзор");
    }
}
