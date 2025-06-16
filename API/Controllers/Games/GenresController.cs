using API.Models.RequestsModels.Games.Genres;
using AutoMapper;
using Data.Repositories.Interfaces;
using Domain.Games;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class GenresController : ControllerBase
{
    private readonly IMapper _mapper;

    private readonly IRepository<Genre> _genresRepository;

    private readonly IValidator<AddGenreModel> _addGenreModelValidator;
    private readonly IValidator<UpdateGenreModel> _updateGenreModelValidator;

    public GenresController(IMapper mapper, IRepository<Genre> genresRepository, IValidator<AddGenreModel> addGenreModelValidator, IValidator<UpdateGenreModel> updateGenreModelValidator)
    {
        _mapper = mapper;
        _genresRepository = genresRepository;
        _addGenreModelValidator = addGenreModelValidator;
        _updateGenreModelValidator = updateGenreModelValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genre>>> GetAllAsync()
    {
        var developers = await _genresRepository.GetAllAsync();

        return Ok(developers);
    }

    [HttpPost]
    public async Task<ActionResult<Genre>> AddAsync(AddGenreModel addGenreModel)
    {
        var validationResult = _addGenreModelValidator.Validate(addGenreModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult);
        }

        var genre = _mapper.Map<Genre>(addGenreModel);

        var insertedGenreId = await _genresRepository.AddAsync(genre);

        genre.Id = insertedGenreId;
        return Created($"api/developers/{genre.Id}", genre);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Genre>> GetAsync(long id)
    {
        var genre = await _genresRepository.GetAsync(id);
        if (genre is null)
            return NotFound();
        else
            return Ok(genre);
    }

    [HttpDelete("{id}")]
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

    [HttpPut("{id}")]
    public async Task<ActionResult<Genre>> UpdateAsync(long id, UpdateGenreModel updateGenreModel)
    {
        var validationResult = _updateGenreModelValidator.Validate(updateGenreModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult);
        }

        var genreToUpdate = await _genresRepository.GetAsync(id);
        if (genreToUpdate is null)
            return NotFound();

        // Map the update model to the existing entity
        var genreToGetAfterUpdate = _mapper.Map<Genre>(updateGenreModel);

        // Update and return the updated entity
        var updatedGenre = await _genresRepository.UpdateAsync(genreToGetAfterUpdate, id);

        return Ok(updatedGenre);
    }
}
