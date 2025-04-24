using API.Models.RequestsModels;
using AutoMapper;
using Data.Repositories.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DevelopersController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    private readonly IRepository<Developer> _developersRepository;

    public DevelopersController(IConfiguration configuration, IMapper mapper, IRepository<Developer> developersRepository)
    {
        _configuration = configuration;
        _mapper = mapper;

        _developersRepository = developersRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Developer>>> GetAllAsync()
    {
        var developers = await _developersRepository.GetAllAsync();

        return Ok(developers);
    }

    [HttpPost]
    public async Task<ActionResult<Developer>> AddAsync(AddDeveloperModel addDeveloperModel)
    {
        var developer = _mapper.Map<Developer>(addDeveloperModel);

        var insertedDeveloperId = await _developersRepository.AddAsync(developer);

        developer.Id = insertedDeveloperId;
        return Created($"api/developers/{developer.Id}", developer);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Developer>> GetAsync(long id)
    {
        var developer = await _developersRepository.GetAsync(id);
        if (developer is null)
            return NotFound();
        else
            return Ok(developer);
    }

    [HttpDelete("{id}")]
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

    [HttpPut("{id}")]
    public async Task<ActionResult<Developer>> UpdateAsync(long id, UpdateDeveloperModel updateDeveloperModel)
    {
        var developerToUpdate = await _developersRepository.GetAsync(id);
        if (developerToUpdate is null)
            return NotFound();

        // Map the update model to the existing entity
        _mapper.Map(updateDeveloperModel, developerToUpdate);

        // Update and return the updated entity
        var updatedDeveloper = await _developersRepository.UpdateAsync(developerToUpdate, id);

        return Ok(updatedDeveloper);
    }
}
