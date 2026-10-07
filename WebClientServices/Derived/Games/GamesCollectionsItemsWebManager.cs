using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;

namespace WebManagers.Derived.Games;

public sealed class GamesCollectionsItemsWebManager : CrudWebManager<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel>
{
    public GamesCollectionsItemsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Games/CollectionsItems")
    {
    }
}
