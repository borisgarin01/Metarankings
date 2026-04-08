using Data.Repositories.Interfaces;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;

namespace API.Controllers.Games;

[ApiController]
[Route("api/games/[controller]")]
[Authorize(Policy = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class CollectionsItemsController : ControllerBase
{
    private IRepository<GameCollectionItem, AddGameCollectionItemModel, UpdateGameCollectionItemModel> _gamesCollectionsItemsRepository;

    public CollectionsItemsController(IRepository<GameCollectionItem, AddGameCollectionItemModel, UpdateGameCollectionItemModel> gamesCollectionsItemsRepository)
    {
        _gamesCollectionsItemsRepository = gamesCollectionsItemsRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<GameCollectionItem>>> GetAllAsync()
    {
        try
        {
            IEnumerable<GameCollectionItem> gameCollectionItems = await _gamesCollectionsItemsRepository.GetAllAsync();
            return Ok(gameCollectionItems);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPost]
    public async Task<ActionResult<long>> AddAsync(AddGameCollectionItemModel addGameCollectionItemModel)
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
        GameCollectionItem gameCollectionItem = await _gamesCollectionsItemsRepository.GetAsync(gameCollectionItemId);
        if (gameCollectionItem is null)
            return NotFound();
        try
        {
            await _gamesCollectionsItemsRepository.RemoveAsync(gameCollectionItemId);
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPut("{gameCollectionItemId:long}")]
    public async Task<ActionResult<GameCollectionItem>> UpdateAsync(long gameCollectionItemId, UpdateGameCollectionItemModel updateGameCollectionItemModel)
    {
        GameCollectionItem gameCollectionItemToUpdate = await _gamesCollectionsItemsRepository.GetAsync(gameCollectionItemId);
        if (gameCollectionItemToUpdate is null)
            return NotFound();
        try
        {
            GameCollectionItem updatedGameCollectionItem = await _gamesCollectionsItemsRepository.UpdateAsync(updateGameCollectionItemModel, gameCollectionItemId);
            return Ok(updatedGameCollectionItem);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}
