using Domain.Games;
using Domain.RequestsModels.Games.Developers;
using WebManagers;

namespace BlazorClient.Pages.Games.Admin.Developers;

public partial class RemoveDeveloperPage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public Developer Developer { get; private set; }

    [Inject]
    public IWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel> DevelopersWebManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Developer = await DevelopersWebManager.GetAsync(Id);
    }
}
