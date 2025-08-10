using Data.Repositories.Classes.Derived.Games;
using Domain.Games;
using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.Reviews;
using IdentityLibrary.DTOs;
using IdentityLibrary.Telegram;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesGamersReviewsController : ControllerBase
{
    private readonly GamesPlayersReviewsRepository _gamesPlayersReviewsRepository;
    private readonly GamesRepository _gamesRepository;

    private readonly UserManager<ApplicationUser> _usersManager;

    private readonly IMapper _mapper;

    private readonly TelegramAuthenticator _telegramAuthenticator;

    public GamesGamersReviewsController(GamesPlayersReviewsRepository gamesPlayersReviewsRepository, TelegramAuthenticator telegramAuthenticator, GamesRepository gamesRepository, UserManager<ApplicationUser> usersManager, IMapper mapper)
    {
        _gamesPlayersReviewsRepository = gamesPlayersReviewsRepository;
        _telegramAuthenticator = telegramAuthenticator;
        _gamesRepository = gamesRepository;
        _usersManager = usersManager;
        _mapper = mapper;
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

        var gameReview = _mapper.Map<GameReview>(addGameReviewModel);

        gameReview.UserId = userId;
        gameReview.Date = DateTime.UtcNow;

        var gameReviewId = await _gamesPlayersReviewsRepository.AddAsync(gameReview);
        var createdGameReview = await _gamesPlayersReviewsRepository.GetAsync(gameReviewId);
        await _telegramAuthenticator.SendMessageAsync($"New game review for game {game.Name} at {this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api/GamesGamersReviews/{createdGameReview.Id}");
        return Created($"api/GamesReviews/{gameReview.Id}", createdGameReview);

    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<GameReview>> GetReview(long id)
    {
        GameReview gameReview = await _gamesPlayersReviewsRepository.GetAsync(id);
        if (gameReview is null)
            return NotFound();
        return Ok(gameReview);
    }
}
