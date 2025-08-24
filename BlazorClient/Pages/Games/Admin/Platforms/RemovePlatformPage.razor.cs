using Domain.Games;
using Domain.RequestsModels.Games.Platforms;
using WebManagers;

namespace BlazorClient.Pages.Games.Admin.Platforms;

public partial class RemovePlatformPage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }
    public Platform Platform { get; private set; }
    
    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> PlatformsWebManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Platform = await PlatformsWebManager.GetAsync(Id);
    }
}
