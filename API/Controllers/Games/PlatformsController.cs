using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Platforms;

namespace API.Controllers.Games;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Games })]
[Route("api/games/[controller]")]
public sealed class PlatformsController : CrudControllerBase<Platform, AddPlatformModel, UpdatePlatformModel>
{
    public PlatformsController(IRepository<Platform, AddPlatformModel, UpdatePlatformModel> platformsRepository) : base(platformsRepository)
    {
    }
}
