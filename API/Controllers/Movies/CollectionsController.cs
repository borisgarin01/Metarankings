using Data.Repositories.Interfaces;
using Domain.Movies.MoviesCollections;
using Domain.RequestsModels.Movies.Collections;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/movies/[controller]")]
[Authorize(Policy = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class CollectionsController : ControllerBase
{
    private readonly IRepository<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel> _moviesCollectionsRepository;
    private readonly IRepository<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel> _moviesCollectionsItemsRepository;

    public CollectionsController(IRepository<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel> moviesCollectionsRepository, IRepository<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel> moviesCollectionsItemsRepository)
    {
        _moviesCollectionsRepository = moviesCollectionsRepository;
        _moviesCollectionsItemsRepository = moviesCollectionsItemsRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MoviesCollection>>> GetAllAsync()
    {
        try
        {
            IEnumerable<MoviesCollection> moviesCollections = await _moviesCollectionsRepository.GetAllAsync();
            return Ok(moviesCollections);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("{offset:long}/{limit:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MoviesCollection>>> GetAllAsync(long offset, long limit)
    {
        try
        {
            IEnumerable<MoviesCollection> gamesCollections = await _moviesCollectionsRepository.GetAsync(offset, limit);
            return Ok(gamesCollections);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("{movieCollectionId:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<MoviesCollection>> GetAsync(long movieCollectionId)
    {
        try
        {
            MoviesCollection moviesCollection = await _moviesCollectionsRepository.GetAsync(movieCollectionId);
            if (moviesCollection is null)
                return NotFound();
            return Ok(moviesCollection);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPost]
    public async Task<ActionResult<long>> AddAsync(AddMoviesCollectionModel addMoviesCollectionModel)
    {
        try
        {
            long insertedMovieCollectionId = await _moviesCollectionsRepository.AddAsync(addMoviesCollectionModel);

            foreach (long selectedMovieId in addMoviesCollectionModel.SelectedMoviesIds)
            {
                _ = await _moviesCollectionsItemsRepository.AddAsync(new AddMoviesCollectionItemModel(selectedMovieId, insertedMovieCollectionId));
            }

            return Ok(insertedMovieCollectionId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpDelete("{gameCollectionId:long}")]
    public async Task<ActionResult<long>> DeleteAsync(long gameCollectionId)
    {
        MoviesCollection collection = await _moviesCollectionsRepository.GetAsync(gameCollectionId);
        if (collection is null)
            return NotFound();
        try
        {
            await _moviesCollectionsRepository.RemoveAsync(gameCollectionId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }


    [HttpPut("{movieCollectionId:long}")]
    public async Task<ActionResult<MoviesCollection>> UpdateAsync(long movieCollectionId, UpdateMoviesCollectionModel updateMoviesCollectionModel)
    {
        MoviesCollection gameCollectionToUpdate = await _moviesCollectionsRepository.GetAsync(movieCollectionId);
        if (gameCollectionToUpdate is null)
            return NotFound();
        try
        {
            MoviesCollection updatedGamesCollection = await _moviesCollectionsRepository.UpdateAsync(updateMoviesCollectionModel, movieCollectionId);
            return Ok(updatedGamesCollection);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}
