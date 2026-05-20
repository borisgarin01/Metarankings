using Blazored.Toast.Services;
using Domain.Games;
using Domain.RequestsModels.Games.Publishers;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Publishers;

public partial class RemovePublisherPage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public Publisher Publisher { get; private set; }

    [Inject]
    public IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel> PublishersWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Publisher = await PublishersWebManager.GetAsync(Id);
    }

    public async Task RemovePublisherAsync()
    {
        HttpResponseMessage httpResponseMessage = await PublishersWebManager.DeleteAsync(Id);
        if (httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/games/publishers/list-publishers");
        else
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
