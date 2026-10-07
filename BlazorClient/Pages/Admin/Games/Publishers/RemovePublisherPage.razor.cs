using Domain.Games;
using Domain.RequestsModels.Games.Publishers;

namespace BlazorClient.Pages.Admin.Games.Publishers;

public partial class RemovePublisherPage : RemoveEntityPageBase<Publisher, AddPublisherModel, UpdatePublisherModel>
{
    protected override string ListUrl => "/admin/games/publishers/list-publishers";
}
