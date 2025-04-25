using API.Models.RequestsModels.Platforms;
using AutoMapper;
using Data.Repositories.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PlatformsController : ControllerBase
{
    private readonly IMapper _mapper;

    private readonly IRepository<Platform> _platformsRepository;

    public PlatformsController(IMapper mapper, IRepository<Platform> platformsRepository)
    {
        _mapper = mapper;
        _platformsRepository = platformsRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Platform>>> GetAllAsync()
    {
        var developers = await _platformsRepository.GetAllAsync();

        return Ok(developers);
    }

    [HttpPost]
    public async Task<ActionResult<Platform>> AddAsync(AddPlatformModel addPlatformModel)
    {
        var platform = _mapper.Map<Platform>(addPlatformModel);

        var insertedGenreId = await _platformsRepository.AddAsync(platform);

        platform.Id = insertedGenreId;
        return Created($"api/developers/{platform.Id}", platform);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Platform>> GetAsync(long id)
    {
        var platform = await _platformsRepository.GetAsync(id);
        if (platform is null)
            return NotFound();
        else
            return Ok(platform);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        var platform = await _platformsRepository.GetAsync(id);
        if (platform is null)
            return NotFound();
        else
        {
            try
            {
                await _platformsRepository.RemoveAsync(platform.Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Platform>> UpdateAsync(long id, UpdatePlatformModel updateGenreModel)
    {
        var platformToUpdate = await _platformsRepository.GetAsync(id);
        if (platformToUpdate is null)
            return NotFound();

        // Map the update model to the existing entity
        var platformToGetAfterUpdate = _mapper.Map<Platform>(updateGenreModel);

        // Update and return the updated entity
        var updatedPlatform = await _platformsRepository.UpdateAsync(platformToGetAfterUpdate, id);

        return Ok(updatedPlatform);
    }
}
