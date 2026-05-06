using Data.Repositories.Interfaces;
using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/movies/[controller]")]
[Authorize(Policy = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class CollectionsItemsController : ControllerBase
{
    private IRepository<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel> _moviesCollectionsItemsRepository;

    public CollectionsItemsController(IRepository<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel> moviesCollectionsItemsRepository)
    {
        _moviesCollectionsItemsRepository = moviesCollectionsItemsRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MoviesCollectionItem>>> GetAllAsync()
    {
        try
        {
            IEnumerable<MoviesCollectionItem> moviesCollectionItems = await _moviesCollectionsItemsRepository.GetAllAsync();
            return Ok(moviesCollectionItems);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPost]
    public async Task<ActionResult<long>> AddAsync(AddMoviesCollectionItemModel addMoviesCollectionItemModel)
    {
        try
        {
            long insertedMovieCollectionItemId = await _moviesCollectionsItemsRepository.AddAsync(addMoviesCollectionItemModel);
            return Ok(insertedMovieCollectionItemId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpDelete("{movieCollectionItemId:long}")]
    public async Task<ActionResult<long>> DeleteAsync(long movieCollectionItemId)
    {
        MoviesCollectionItem moviesCollectionItem = await _moviesCollectionsItemsRepository.GetAsync(movieCollectionItemId);
        if (moviesCollectionItem is null)
            return NotFound();
        try
        {
            await _moviesCollectionsItemsRepository.RemoveAsync(movieCollectionItemId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPut("{movieCollectionItemId:long}")]
    public async Task<ActionResult<MoviesCollectionItem>> UpdateAsync(long movieCollectionItemId, UpdateMoviesCollectionItemModel updateMoviesCollectionItemModel)
    {
        MoviesCollectionItem movieCollectionItemToUpdate = await _moviesCollectionsItemsRepository.GetAsync(movieCollectionItemId);
        if (movieCollectionItemToUpdate is null)
            return NotFound();
        try
        {
            MoviesCollectionItem updatedMovieCollectionItem = await _moviesCollectionsItemsRepository.UpdateAsync(updateMoviesCollectionItemModel, movieCollectionItemId);
            return Ok(updatedMovieCollectionItem);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}
