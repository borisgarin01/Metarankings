using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/[controller]")]
public class MoviesDirectorsController : ControllerBase
{
    private readonly IRepository<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel> _moviesDirectorsRepository;
    private readonly IMapper _mapper;

    public MoviesDirectorsController(IRepository<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel> moviesDirectorsRepository, IMapper mapper)
    {
        _moviesDirectorsRepository = moviesDirectorsRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<MovieDirector>> GetAllAsync()
    {
        try
        {
            var moviesDirectors = await _moviesDirectorsRepository.GetAllAsync();
            return Ok(moviesDirectors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<MovieDirector>> GetAsync(long id)
    {
        try
        {
            var movieDirector = await _moviesDirectorsRepository.GetAsync(id);
            if (movieDirector is null)
            {
                return NotFound();
            }
            return Ok(movieDirector);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<MovieDirector>> AddAsync(AddMovieDirectorModel addMovieDirectorModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var insertedId = await _moviesDirectorsRepository.AddAsync(addMovieDirectorModel);

                var insertedMovieDirector = await _moviesDirectorsRepository.GetAsync(insertedId);

                return Created($"/api/moviesdirectors/{insertedId}", insertedMovieDirector);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        return BadRequest();
    }

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<MovieDirector>> UpdateAsync(long id, UpdateMovieDirectorModel updateMovieDirectorModel)
    {
        var movieDirectorToUpdate = await _moviesDirectorsRepository.GetAsync(id);

        if (movieDirectorToUpdate is null)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var updatedMovieDirector = await _moviesDirectorsRepository.UpdateAsync(updateMovieDirectorModel, id);

                return Ok(updatedMovieDirector);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        return BadRequest();
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<MovieDirector>> UpdateAsync(long id)
    {
        var movieDirectorToUpdate = await _moviesDirectorsRepository.GetAsync(id);

        if (movieDirectorToUpdate is null)
            return NotFound();

        await _moviesDirectorsRepository.RemoveAsync(id);

        return NoContent();
    }
}
