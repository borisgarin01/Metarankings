using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using IdentityLibrary.Telegram;

namespace API.Controllers.Games;

[ApiController]
[Route("api/games/[controller]")]
public sealed class GenresController : ControllerBase
{
    private readonly IRepository<Genre, AddGenreModel, UpdateGenreModel> _genresRepository;

    private readonly TelegramAuthenticator _telegramAuthenticator;

    public GenresController(IRepository<Genre, AddGenreModel, UpdateGenreModel> genresRepository, TelegramAuthenticator telegramAuthenticator)
    {
        _genresRepository = genresRepository;
        _telegramAuthenticator = telegramAuthenticator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genre>>> GetAllAsync()
    {
        var genres = await _genresRepository.GetAllAsync();

        return Ok(genres);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<Genre>> AddAsync(AddGenreModel addGenreModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(addGenreModel);
        }

        var insertedGenreId = await _genresRepository.AddAsync(addGenreModel);

        await _telegramAuthenticator.SendMessageAsync($"New genre {addGenreModel.Name} at {this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/games/genres/{insertedGenreId}");

        Genre insertedGenre = await _genresRepository.GetAsync(insertedGenreId);

        return Created($"api/games/genres/{insertedGenreId}", insertedGenre);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Genre>> GetAsync(long id)
    {
        var genre = await _genresRepository.GetAsync(id);
        if (genre is null)
            return NotFound();
        else
            return Ok(genre);
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        var developer = await _genresRepository.GetAsync(id);
        if (developer is null)
            return NotFound();
        else
        {
            try
            {
                await _genresRepository.RemoveAsync(developer.Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<Genre>> UpdateAsync(long id, UpdateGenreModel updateGenreModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var genreToUpdate = await _genresRepository.GetAsync(id);
        if (genreToUpdate is null)
            return NotFound();

        // Update and return the updated entity
        var updatedGenre = await _genresRepository.UpdateAsync(updateGenreModel, id);

        return Ok(updatedGenre);
    }
}
