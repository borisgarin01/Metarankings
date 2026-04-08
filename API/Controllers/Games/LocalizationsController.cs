using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.Localizations;
using IdentityLibrary.Telegram;

namespace API.Controllers.Games;

[ApiController]
[Route("api/games/[controller]")]
public sealed class LocalizationsController : ControllerBase
{
    private readonly ILocalizationsRepository _localizationsRepository;

    private readonly TelegramAuthenticator _telegramAuthenticator;

    public LocalizationsController(ILocalizationsRepository localizationsRepository, TelegramAuthenticator telegramAuthenticator)
    {
        _localizationsRepository = localizationsRepository;
        _telegramAuthenticator = telegramAuthenticator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Localization>>> GetAllAsync()
    {
        IEnumerable<Localization> developers = await _localizationsRepository.GetAllAsync();

        return Ok(developers);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<Localization>> AddAsync(AddLocalizationModel addLocalizationModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        long insertedLocalizationId = await _localizationsRepository.AddAsync(addLocalizationModel);

        Localization insertedLocalization = await _localizationsRepository.GetAsync(insertedLocalizationId);

        await _telegramAuthenticator.SendMessageAsync($"New localization {addLocalizationModel.Name} at {Request.Scheme}://{Request.Host}{Request.PathBase}/games/localizations/{insertedLocalizationId}");
        return Created($"api/games/localizations/{insertedLocalizationId}", insertedLocalization);
    }

    [HttpGet("{id:long}")]
    [HttpGet("{id:long}/{platformId:long}")]
    public async Task<ActionResult<Localization>> GetAsync(long id, long? platformId)
    {
        Localization localization;
        if (!platformId.HasValue)
            localization = await _localizationsRepository.GetAsync(id);
        else
            localization = await _localizationsRepository.GetByPlatformAsync(id, platformId.Value);
        if (localization is null)
            return NotFound();
        else
            return Ok(localization);
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        Localization? localization = await _localizationsRepository.GetAsync(id);
        if (localization is null)
            return NotFound();
        else
        {
            try
            {
                await _localizationsRepository.RemoveAsync(localization.Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<Localization>> UpdateAsync(long id, UpdateLocalizationModel updateLocalizationModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Localization? localizationToUpdate = await _localizationsRepository.GetAsync(id);
        if (localizationToUpdate is null)
            return NotFound();

        // Update and return the updated entity
        Localization updatedLocalization = await _localizationsRepository.UpdateAsync(updateLocalizationModel, id);

        return Ok(updatedLocalization);
    }
}
