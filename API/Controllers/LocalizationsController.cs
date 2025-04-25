using API.Models.RequestsModels.Localizations;
using AutoMapper;
using Data.Repositories.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class LocalizationsController : ControllerBase
{
    private readonly IMapper _mapper;

    private readonly IRepository<Localization> _localizationsRepository;

    public LocalizationsController(IMapper mapper, IRepository<Localization> localizationsRepository)
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
    public async Task<ActionResult<Localization>> AddAsync(AddLocalizationModel addLocalizationModel)
    {
        var localization = _mapper.Map<Localization>(addLocalizationModel);

        var insertedLocalizationId = await _localizationsRepository.AddAsync(localization);

        localization.Id = insertedLocalizationId;
        return Created($"api/developers/{localization.Id}", localization);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Localization>> GetAsync(long id)
    {
        var localization = await _localizationsRepository.GetAsync(id);
        if (localization is null)
            return NotFound();
        else
            return Ok(localization);
    }

    [HttpDelete("{id}")]
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

    [HttpPut("{id}")]
    public async Task<ActionResult<Localization>> UpdateAsync(long id, UpdateLocalizationModel updateLocalizationModel)
    {
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
