using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.Localizations;

namespace API.Controllers.Games;

[Route("api/games/[controller]")]
public sealed class LocalizationsController : CrudControllerBase<Localization, AddLocalizationModel, UpdateLocalizationModel>
{
    private readonly ILocalizationsRepository _localizationsRepository;

    public LocalizationsController(ILocalizationsRepository localizationsRepository) : base(localizationsRepository)
    {
        _localizationsRepository = localizationsRepository;
    }

    [HttpGet("{id:long}/{platformId:long}")]
    public async Task<ActionResult<Localization>> GetByPlatformAsync(long id, long platformId)
    {
        Localization? localization = await _localizationsRepository.GetByPlatformAsync(id, platformId);
        if (localization is null)
            return NotFound();

        return Ok(localization);
    }
}
