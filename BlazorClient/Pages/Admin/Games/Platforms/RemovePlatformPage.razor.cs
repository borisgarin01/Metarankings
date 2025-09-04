using Domain.Games;
using Domain.RequestsModels.Games.Platforms;
using WebManagers;
using WebManagers.Derived;

namespace BlazorClient.Pages.Admin.Games.Platforms;

public partial class RemovePlatformPage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }
    public Platform Platform { get; private set; }
    
    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> PlatformsWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Platform = await PlatformsWebManager.GetAsync(Id);
    }

    public async Task RemovePlatformAsync()
    {
        HttpResponseMessage httpResponseMessage = await PlatformsWebManager.DeleteAsync(Id);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            NavigationManager.NavigateTo("/admin/platforms/list-platforms");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}
