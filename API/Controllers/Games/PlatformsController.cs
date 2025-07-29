using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Platforms;

namespace API.Controllers.Games;

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
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<Platform>> AddAsync(AddPlatformModel addPlatformModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var platform = _mapper.Map<Platform>(addPlatformModel);

        var insertedPlatformId = await _platformsRepository.AddAsync(platform);

        platform = platform with { Id = insertedPlatformId };
        return Created($"api/developers/{platform.Id}", platform);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Platform>> GetAsync(long id)
    {
        var platform = await _platformsRepository.GetAsync(id);
        if (platform is null)
            return NotFound();
        else
            return Ok(platform);
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
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

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<Platform>> UpdateAsync(long id, UpdatePlatformModel updatePlatformModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var platformToUpdate = await _platformsRepository.GetAsync(id);
        if (platformToUpdate is null)
            return NotFound();

        // Map the update model to the existing entity
        var platformToGetAfterUpdate = _mapper.Map<Platform>(updatePlatformModel);

        // Update and return the updated entity
        var updatedPlatform = await _platformsRepository.UpdateAsync(platformToGetAfterUpdate, id);

        return Ok(updatedPlatform);
    }
}
