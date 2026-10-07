using Data.Repositories.Interfaces;
using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;

namespace API.Controllers.Movies;

[Route("api/movies/[controller]")]
public sealed class CollectionsController : CrudControllerBase<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel>
{
    public CollectionsController(IRepository<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel> moviesCollectionsRepository) : base(moviesCollectionsRepository)
    {
    }
}
