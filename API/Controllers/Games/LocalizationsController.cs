﻿using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.Localizations;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class LocalizationsController : ControllerBase
{
    private readonly IMapper _mapper;

    private readonly ILocalizationsRepository _localizationsRepository;

    public LocalizationsController(IMapper mapper, ILocalizationsRepository localizationsRepository)
    {
        _mapper = mapper;
        _localizationsRepository = localizationsRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Localization>>> GetAllAsync()
    {
        var developers = await _localizationsRepository.GetAllAsync();

        return Ok(developers);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<Localization>> AddAsync(AddLocalizationModel addLocalizationModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var localization = _mapper.Map<Localization>(addLocalizationModel);

        var insertedLocalizationId = await _localizationsRepository.AddAsync(localization);

        localization = localization with { Id = insertedLocalizationId };
        return Created($"api/developers/{localization.Id}", localization);
    }

    [HttpGet("{id:long}")]
    [HttpGet("{id:long}/{platformId:long}")]
    public async Task<ActionResult<Localization>> GetAsync(long id, long? platformId)
    {
        Localization localization = null;

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
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        var localization = await _localizationsRepository.GetAsync(id);
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
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
    public async Task<ActionResult<Localization>> UpdateAsync(long id, UpdateLocalizationModel updateLocalizationModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var localizationToUpdate = await _localizationsRepository.GetAsync(id);
        if (localizationToUpdate is null)
            return NotFound();

        // Map the update model to the existing entity
        var localizationToGetAfterUpdate = _mapper.Map<Localization>(updateLocalizationModel);

        // Update and return the updated entity
        var updatedLocalization = await _localizationsRepository.UpdateAsync(localizationToGetAfterUpdate, id);

        return Ok(updatedLocalization);
    }
}
