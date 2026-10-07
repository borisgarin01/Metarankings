using Domain.Games;
using Domain.RequestsModels.Games.Developers;

namespace BlazorClient.Pages.Admin.Games.Developers;

public partial class RemoveDeveloperPage : RemoveEntityPageBase<Developer, AddDeveloperModel, UpdateDeveloperModel>
{
    protected override string ListUrl => "/admin/games/developers/list-developers";
}
