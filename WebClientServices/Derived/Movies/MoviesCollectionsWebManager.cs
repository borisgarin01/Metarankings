using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;

namespace WebManagers.Derived.Movies;

public sealed class MoviesCollectionsWebManager : CrudWebManager<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel>
{
    public MoviesCollectionsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Movies/Collections")
    {
    }
}
