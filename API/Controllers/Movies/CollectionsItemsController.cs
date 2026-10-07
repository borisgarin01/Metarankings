using Data.Repositories.Interfaces;
using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;

namespace API.Controllers.Movies;

[Route("api/movies/[controller]")]
public sealed class CollectionsItemsController : CrudControllerBase<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel>
{
    public CollectionsItemsController(IRepository<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel> moviesCollectionsItemsRepository) : base(moviesCollectionsItemsRepository)
    {
    }
}
