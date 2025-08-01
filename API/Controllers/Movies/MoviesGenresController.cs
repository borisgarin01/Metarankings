﻿using Data.Repositories.Interfaces;
using Domain.Movies;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/[controller]")]
public class MoviesGenresController : ControllerBase
{
    private readonly IRepository<MovieGenre> _moviesGenresRepository;

    public MoviesGenresController(IRepository<MovieGenre> moviesGenresRepository)
    {
        _moviesGenresRepository = moviesGenresRepository;
    }

    [HttpGet]
    public async Task<ActionResult<MovieGenre>> GetAllAsync()
    {
        try
        {
            var moviesDirectors = await _moviesGenresRepository.GetAllAsync();
            return Ok(moviesDirectors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<MovieGenre>> GetAsync(long id)
    {
        try
        {
            var movieDirector = await _moviesGenresRepository.GetAsync(id);
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
    public async Task<ActionResult<MovieGenre>> AddAsync(MovieGenre movieGenre)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var insertedId = await _moviesGenresRepository.AddAsync(movieGenre);

                movieGenre = movieGenre with { Id = insertedId };

                return Ok(movieGenre);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        return BadRequest();
    }
}

