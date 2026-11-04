using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.Platforms;

namespace API.Controllers.Games;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Games })]
[Route("api/games/[controller]")]
public sealed class PlatformsController : CrudControllerBase<Platform, AddPlatformModel, UpdatePlatformModel>
{
    private readonly PlatformsRepository _platformsRepository;
    public PlatformsController(PlatformsRepository platformsRepository) : base(platformsRepository)
    {
        _platformsRepository = platformsRepository;
    }

    protected override async Task<ActionResult?> ValidateAddAsync(AddPlatformModel addPlatformModel)
    {
        Platform? existingPlatform = await _platformsRepository.GetByNameAsync(addPlatformModel.Name);

        if (existingPlatform is not null)
            return BadRequest($"Платформа с названием {existingPlatform.Name} уже существует");

        return null;
    }
}
