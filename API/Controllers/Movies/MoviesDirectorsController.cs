using API.Models.RequestsModels.Movies.MoviesDirectors;
using AutoMapper;
using Data.Repositories.Interfaces;
using Domain.Movies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/[controller]")]
public class MoviesDirectorsController : ControllerBase
{
    private readonly IRepository<MovieDirector> _moviesDirectorsRepository;
    private readonly IMapper _mapper;

    public MoviesDirectorsController(IRepository<MovieDirector> moviesDirectorsRepository, IMapper mapper)
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

    [HttpGet("{id}")]
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
    [Authorize]
    public async Task<ActionResult<MovieDirector>> AddAsync(AddMovieDirectorModel addMovieDirectorModel)
    {
        if (ModelState.IsValid)
        {
            var movieDirector = _mapper.Map<MovieDirector>(addMovieDirectorModel);
            try
            {
                var insertedId = await _moviesDirectorsRepository.AddAsync(movieDirector);

                movieDirector.Id = insertedId;

                return Ok(movieDirector);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MovieDirector>> UpdateAsync(long id, UpdateMovieDirectorModel updateMovieDirectorModel)
    {
        var movieDirectorToUpdate = await _moviesDirectorsRepository.GetAsync(id);

        if (movieDirectorToUpdate is null)
            return NotFound();

        if (ModelState.IsValid)
        {
            var movieDirector = _mapper.Map<MovieDirector>(updateMovieDirectorModel);
            try
            {
                var updatedMovieDirector = await _moviesDirectorsRepository.UpdateAsync(movieDirector, id);

                return Ok(movieDirector);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        return BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<MovieDirector>> UpdateAsync(long id)
    {
        var movieDirectorToUpdate = await _moviesDirectorsRepository.GetAsync(id);

        if (movieDirectorToUpdate is null)
            return NotFound();

        await _moviesDirectorsRepository.RemoveAsync(id);

        return NoContent();
    }
}
