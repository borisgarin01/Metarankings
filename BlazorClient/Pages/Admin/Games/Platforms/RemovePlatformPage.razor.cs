using Domain.Games;
using Domain.RequestsModels.Games.Platforms;

namespace BlazorClient.Pages.Admin.Games.Platforms;

public partial class RemovePlatformPage : RemoveEntityPageBase<Platform, AddPlatformModel, UpdatePlatformModel>
{
    protected override string ListUrl => "/admin/games/platforms/list-platforms";
}
