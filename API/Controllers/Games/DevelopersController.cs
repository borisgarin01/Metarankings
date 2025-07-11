using API.Models.RequestsModels.Games.Developers;
using AutoMapper;
using Data.Repositories.Interfaces;
using Domain.Games;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class DevelopersController : ControllerBase
{
    private readonly IMapper _mapper;

    private readonly IRepository<Developer> _developersRepository;

    private readonly IValidator<AddDeveloperModel> _addDeveloperModelValidator;
    private readonly IValidator<UpdateDeveloperModel> _updateDeveloperModelValidator;

    public DevelopersController(IMapper mapper, IRepository<Developer> developersRepository, IValidator<AddDeveloperModel> addDeveloperModelValidator, IValidator<UpdateDeveloperModel> updateDeveloperModelValidator)
    {
        _mapper = mapper;

        _developersRepository = developersRepository;
        _addDeveloperModelValidator = addDeveloperModelValidator;
        _updateDeveloperModelValidator = updateDeveloperModelValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Developer>>> GetAllAsync()
    {
        var developers = await _developersRepository.GetAllAsync();

        return Ok(developers);
    }

    [HttpGet("{offset:long}/{limit:long}")]
    public async Task<ActionResult<IEnumerable<Developer>>> GetAsync(long offset, long limit)
    {
        var developers = await _developersRepository.GetAsync(offset, limit);

        return Ok(developers);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<Developer>> AddAsync(AddDeveloperModel addDeveloperModel)
    {
        var validationResult = _addDeveloperModelValidator.Validate(addDeveloperModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult);
        }

        var developer = _mapper.Map<Developer>(addDeveloperModel);

        var insertedDeveloperId = await _developersRepository.AddAsync(developer);

        developer.Id = insertedDeveloperId;
        return Created($"api/developers/{developer.Id}", developer);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Developer>> GetAsync(long id)
    {
        var developer = await _developersRepository.GetAsync(id);
        if (developer is null)
            return NotFound();
        else
            return Ok(developer);
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        var developer = await _developersRepository.GetAsync(id);
        if (developer is null)
            return NotFound();
        else
        {
            try
            {
                await _developersRepository.RemoveAsync(developer.Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<Developer>> UpdateAsync(long id, UpdateDeveloperModel updateDeveloperModel)
    {
        var validationResult = _updateDeveloperModelValidator.Validate(updateDeveloperModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult);
        }

        var developerToUpdate = await _developersRepository.GetAsync(id);
        if (developerToUpdate is null)
            return NotFound();

        // Map the update model to the existing entity
        var developerToGetAfterUpdate = _mapper.Map<Developer>(updateDeveloperModel);

        // Update and return the updated entity
        var updatedDeveloper = await _developersRepository.UpdateAsync(developerToGetAfterUpdate, id);

        return Ok(updatedDeveloper);
    }
}
