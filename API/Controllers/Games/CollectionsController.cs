using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Interfaces;
using Domain.Games.Collections;

namespace API.Controllers.Games;

[ApiController]
[Route("api/games/[controller]")]
[Authorize(Policy = "Admin", AuthenticationSchemes = "Bearer")]
public sealed class CollectionsController : ControllerBase
{
    private readonly IRepository<GameCollection, AddGameCollectionModel, UpdateGameCollectionModel> _gamesCollectionsRepository;

    public CollectionsController(IRepository<GameCollection, AddGameCollectionModel, UpdateGameCollectionModel> gamesCollectionsRepository)
    {
        _gamesCollectionsRepository = gamesCollectionsRepository;
    }

    [HttpGet]
    [AllowAnonymous]
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
            long insertedGameCollectionId = await _gamesCollectionsRepository.AddAsync(addGameCollectionModel);
            return Ok(insertedGameCollectionId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpDelete("{gameCollectionId:long}")]
    public async Task<ActionResult<long>> DeleteAsync(long gameCollectionId)
    {
        GameCollection collection = await _gamesCollectionsRepository.GetAsync(gameCollectionId);
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
    public async Task<ActionResult<GameCollection>> UpdateAsync(long gameCollectionId, UpdateGameCollectionModel updateGameCollectionModel)
    {
        GameCollection gameCollectionToUpdate = await _gamesCollectionsRepository.GetAsync(gameCollectionId);
        if (gameCollectionToUpdate is null)
            return NotFound();
        try
        {
            GameCollection updatedGamesCollection = await _gamesCollectionsRepository.UpdateAsync(updateGameCollectionModel, gameCollectionId);
            return Ok(updateGameCollectionModel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}
