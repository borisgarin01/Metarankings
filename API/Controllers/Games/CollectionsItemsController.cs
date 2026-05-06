using Data.Repositories.Interfaces;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;

namespace API.Controllers.Games;

[ApiController]
[Route("api/games/[controller]")]
[Authorize(Policy = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class CollectionsItemsController : ControllerBase
{
    private IRepository<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel> _gamesCollectionsItemsRepository;

    public CollectionsItemsController(IRepository<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel> gamesCollectionsItemsRepository)
    {
        _gamesCollectionsItemsRepository = gamesCollectionsItemsRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<GamesCollectionItem>>> GetAllAsync()
    {
        try
        {
            IEnumerable<GamesCollectionItem> gameCollectionItems = await _gamesCollectionsItemsRepository.GetAllAsync();
            return Ok(gameCollectionItems);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPost]
    public async Task<ActionResult<long>> AddAsync(AddGamesCollectionItemModel addGameCollectionItemModel)
    {
        try
        {
            long insertedGameCollectionItemId = await _gamesCollectionsItemsRepository.AddAsync(addGameCollectionItemModel);
            return Ok(insertedGameCollectionItemId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpDelete("{gameCollectionItemId:long}")]
    public async Task<ActionResult<long>> DeleteAsync(long gameCollectionItemId)
    {
        GamesCollectionItem gameCollectionItem = await _gamesCollectionsItemsRepository.GetAsync(gameCollectionItemId);
        if (gameCollectionItem is null)
            return NotFound();
        try
        {
            await _gamesCollectionsItemsRepository.RemoveAsync(gameCollectionItemId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPut("{gameCollectionItemId:long}")]
    public async Task<ActionResult<GamesCollectionItem>> UpdateAsync(long gameCollectionItemId, UpdateGamesCollectionItemModel updateGameCollectionItemModel)
    {
        GamesCollectionItem gameCollectionItemToUpdate = await _gamesCollectionsItemsRepository.GetAsync(gameCollectionItemId);
        if (gameCollectionItemToUpdate is null)
            return NotFound();
        try
        {
            GamesCollectionItem updatedGameCollectionItem = await _gamesCollectionsItemsRepository.UpdateAsync(updateGameCollectionItemModel, gameCollectionItemId);
            return Ok(updatedGameCollectionItem);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}
