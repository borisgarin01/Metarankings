using Domain.Games;
using Domain.RequestsModels.Games.Publishers;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Publishers;

public partial class AddPublisherPage : ComponentBase
{

    [Inject]
    public IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel> PublishersWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public AddPublisherModel AddPublisherModel { get; } = new AddPublisherModel();

    public async Task AddPublisherAsync()
    {
        HttpResponseMessage httpResponseMessage = await PublishersWebManager.AddAsync(AddPublisherModel);
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/Games/publishers/list-publishers");
        else
            if (httpResponseMessage is not null)
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
    }

}
