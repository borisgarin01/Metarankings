using Domain.Games;
using Domain.RequestsModels.Games.Localizations;
using Domain.RequestsModels.Games.Publishers;
using WebManagers;

namespace BlazorClient.Pages.Games.Admin.Publishers;

public partial class RemovePublisherPage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public Publisher Publisher { get; private set; }

    [Inject]
    public IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel> PublishersWebManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Publisher = await PublishersWebManager.GetAsync(Id);
    }
}
