using Data.Repositories.Interfaces;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;

namespace API.Controllers.Games;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Games })]
[Route("api/games/[controller]")]
public sealed class CollectionsItemsController : CrudControllerBase<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel>
{
    public CollectionsItemsController(IRepository<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel> gamesCollectionsItemsRepository) : base(gamesCollectionsItemsRepository)
    {
    }
}
