using Data.Repositories.Interfaces;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;

namespace API.Controllers.Games;

[ApiController]
[Route("api/games/[controller]")]
[Authorize(Policy = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class CollectionsController : ControllerBase
{
    private readonly ILogger<CollectionsController> _logger;
    private readonly IRepository<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel> _gamesCollectionsRepository;
    private readonly IRepository<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel> _gamesCollectionsItemsRepository;

    public CollectionsController(IRepository<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel> gamesCollectionsRepository, IRepository<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel> gamesCollectionsItemsRepository, ILogger<CollectionsController> logger)
    {
        _gamesCollectionsRepository = gamesCollectionsRepository;
        _gamesCollectionsItemsRepository = gamesCollectionsItemsRepository;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<GamesCollection>>> GetAllAsync()
    {
        try
        {
            IEnumerable<GamesCollection> gamesCollections = await _gamesCollectionsRepository.GetAllAsync();
            return Ok(gamesCollections);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("{offset:long}/{limit:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<GamesCollection>>> GetAllAsync(long offset, long limit)
    {
        try
        {
            IEnumerable<GamesCollection> gamesCollections = await _gamesCollectionsRepository.GetAsync(offset, limit);
            return Ok(gamesCollections);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("{gameCollectionId:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<GamesCollection>> GetAsync(long gameCollectionId)
    {
        try
        {
            GamesCollection gamesCollection = await _gamesCollectionsRepository.GetAsync(gameCollectionId);
            return Ok(gamesCollection);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPost]
    public async Task<ActionResult<long>> AddAsync(AddGamesCollectionModel addGameCollectionModel)
    {
        try
        {
            long insertedGameCollectionId = await _gamesCollectionsRepository.AddAsync(addGameCollectionModel);

            return Ok(insertedGameCollectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message, ex.StackTrace);
            return StatusCode(500, ex);
        }
    }

    [HttpDelete("{gameCollectionId:long}")]
    public async Task<ActionResult<long>> DeleteAsync(long gameCollectionId)
    {
        GamesCollection collection = await _gamesCollectionsRepository.GetAsync(gameCollectionId);
        if (collection is null)
            return NotFound();
        try
        {
            await _gamesCollectionsRepository.RemoveAsync(gameCollectionId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }


    [HttpPut("{gameCollectionId:long}")]
    public async Task<ActionResult<GamesCollection>> UpdateAsync(long gameCollectionId, UpdateGamesCollectionModel updateGameCollectionModel)
    {
        GamesCollection gameCollectionToUpdate = await _gamesCollectionsRepository.GetAsync(gameCollectionId);
        if (gameCollectionToUpdate is null)
            return NotFound();
        try
        {
            GamesCollection updatedGamesCollection = await _gamesCollectionsRepository.UpdateAsync(updateGameCollectionModel, gameCollectionId);
            return Ok(updateGameCollectionModel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}
