using Domain.Games;
using Domain.RequestsModels.Games.Developers;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Developers;

public partial class RemoveDeveloperPage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public Developer Developer { get; private set; }
    
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public IWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel> DevelopersWebManager { get; set; }



    protected override async Task OnInitializedAsync()
    {
        Developer = await DevelopersWebManager.GetAsync(Id);
    }

    public async Task RemoveDeveloperAsync()
    {
        HttpResponseMessage httpResponseMessage = await DevelopersWebManager.DeleteAsync(Id);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            NavigationManager.NavigateTo("/admin/games/developers/list-developers");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}
