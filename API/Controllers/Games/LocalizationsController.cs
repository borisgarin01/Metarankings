using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.Localizations;

namespace API.Controllers.Games;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Games })]
[Route("api/games/[controller]")]
public sealed class LocalizationsController : CrudControllerBase<Localization, AddLocalizationModel, UpdateLocalizationModel>
{
    private readonly LocalizationsRepository _localizationsRepository;

    public LocalizationsController(LocalizationsRepository localizationsRepository) : base(localizationsRepository)
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

    protected override async Task<ActionResult?> ValidateAddAsync(AddLocalizationModel addLocalizationModel)
    {
        Localization? existingLocalization = await _localizationsRepository.GetByNameAsync(addLocalizationModel.Name);

        if (existingLocalization is not null)
            return BadRequest($"Локализация с названием {existingLocalization.Name} уже существует");

        return null;
    }
}
