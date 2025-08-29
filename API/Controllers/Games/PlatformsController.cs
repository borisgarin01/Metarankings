using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Platforms;
using IdentityLibrary.Telegram;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class PlatformsController : ControllerBase
{
    private readonly IRepository<Platform, AddPlatformModel, UpdatePlatformModel> _platformsRepository;

    private readonly TelegramAuthenticator _telegramAuthenticator;

    public PlatformsController(IRepository<Platform, AddPlatformModel, UpdatePlatformModel> platformsRepository, TelegramAuthenticator telegramAuthenticator)
    {
        _platformsRepository = platformsRepository;
        _telegramAuthenticator = telegramAuthenticator;
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

        var insertedPlatformId = await _platformsRepository.AddAsync(addPlatformModel);

        var insertedPlatform = await _platformsRepository.GetAsync(insertedPlatformId);

        await _telegramAuthenticator.SendMessageAsync($"New platform {addPlatformModel.Name} at {this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api/platforms/{insertedPlatformId}");
        return Created($"api/platforms/{insertedPlatform.Id}", insertedPlatform);
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

        // Update and return the updated entity
        var updatedPlatform = await _platformsRepository.UpdateAsync(updatePlatformModel, id);

        return Ok(updatedPlatform);
    }
}
