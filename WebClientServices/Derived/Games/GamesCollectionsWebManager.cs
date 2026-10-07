using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;

namespace WebManagers.Derived.Games;

public sealed class GamesCollectionsWebManager : CrudWebManager<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel>
{
    public GamesCollectionsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Games/Collections")
    {
    }
}
