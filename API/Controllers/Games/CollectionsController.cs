using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Interfaces.Derived;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using ExcelProcessors;

namespace API.Controllers.Games;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Games })]
[Route("api/games/[controller]")]
public sealed class CollectionsController : CrudControllerBase<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel>
{
    private readonly GamesCollectionsRepository _gamesCollectionsRepository;
    
    public CollectionsController(
        GamesCollectionsRepository gamesCollectionsRepository)
        : base(gamesCollectionsRepository)
    {
        _gamesCollectionsRepository = gamesCollectionsRepository;
    }

    [HttpPost("upload-collections-from-json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public Task<ActionResult> AddFromJsonAsync(IEnumerable<AddGamesCollectionModel> collections)
    {
        return AddRangeFromJsonAsync(collections, "GamesCollections");
    }

    protected override async Task<ActionResult?> ValidateAddAsync(AddGamesCollectionModel addGamesCollectionModel)
    {
        GamesCollection? existingCollection = await _gamesCollectionsRepository.GetByNameAsync(addGamesCollectionModel.Name);

        if (existingCollection is not null)
            return BadRequest($"Коллекция с именем {existingCollection.Name} уже существует");

        return null;
    }
}