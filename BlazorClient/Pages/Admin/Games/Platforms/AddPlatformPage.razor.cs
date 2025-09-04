using Domain.Games;
using Domain.RequestsModels.Games.Platforms;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Platforms;

public partial class AddPlatformPage : ComponentBase
{
    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> PlatformsWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public AddPlatformModel AddPlatformModel { get; } = new AddPlatformModel();

    public async Task AddPlatformAsync()
    {
        HttpResponseMessage httpResponseMessage = await PlatformsWebManager.AddAsync(AddPlatformModel);
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/Games/platforms/list-platforms");
        else
            if (httpResponseMessage is not null)
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
