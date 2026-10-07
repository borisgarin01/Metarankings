using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;

namespace WebManagers.Derived.Movies;

public sealed class MoviesCollectionsItemsWebManager : CrudWebManager<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel>
{
    public MoviesCollectionsItemsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Movies/CollectionsItems")
    {
    }
}
