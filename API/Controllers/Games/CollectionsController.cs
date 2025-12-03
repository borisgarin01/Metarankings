using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Interfaces;
using Domain.Games.Collections;

namespace API.Controllers.Games;

[ApiController]
[Route("api/games/[controller]")]
public sealed class CollectionsController : ControllerBase
{
    private readonly IRepository<GameCollection, AddGameCollectionModel, UpdateGameCollectionModel> _gamesCollectionsRepository;

    public CollectionsController(IRepository<GameCollection, AddGameCollectionModel, UpdateGameCollectionModel> gamesCollectionsRepository)
    {
        _gamesCollectionsRepository = gamesCollectionsRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameCollection>>> GetAllAsync()
    {
        try
        {
            IEnumerable<GameCollection> gamesCollections = await _gamesCollectionsRepository.GetAllAsync();
            return Ok(gamesCollections);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPost]
    public async Task<ActionResult<long>> AddAsync(AddGameCollectionModel addGameCollectionModel)
    {
        try
        {
            long insertedCollectionId = await _gamesCollectionsRepository.AddAsync(addGameCollectionModel);
            return Ok(insertedCollectionId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpDelete("{collectionId:long}")]
    public async Task<ActionResult<long>> DeleteAsync(long collectionId)
    {
        GameCollection collection = await _gamesCollectionsRepository.GetAsync(collectionId);
        if (collection is null)
            return NotFound();
        try
        {
            await _gamesCollectionsRepository.RemoveAsync(collectionId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPut("{collectionId:long}")]
    public async Task<ActionResult<GameCollection>> UpdateAsync(long collectionId, UpdateGameCollectionModel updateGameCollectionModel)
    {
        GameCollection gameCollectionToUpdate = await _gamesCollectionsRepository.GetAsync(collectionId);
        if (gameCollectionToUpdate is null)
            return NotFound();
        try
        {
            GameCollection updatedGamesCollection = await _gamesCollectionsRepository.UpdateAsync(updateGameCollectionModel, collectionId);
            return Ok(updateGameCollectionModel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}
