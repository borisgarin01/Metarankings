using Data.Repositories.Interfaces;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;

namespace API.Controllers.Games;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Games })]
[Route("api/games/[controller]")]
public sealed class CollectionsController : CrudControllerBase<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel>
{
    public CollectionsController(IRepository<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel> gamesCollectionsRepository) : base(gamesCollectionsRepository)
    {
    }
}
