using Domain.RequestsModels.Games.Publishers;
using WebManagers;
using Domain.Games;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Admin.Games.Publishers;

[Authorize(Policy = "Admin")]
public partial class ListPublishersPage : ComponentBase
{
    private IEnumerable<Publisher> publishers;

    public IEnumerable<Publisher> Publishers
    {
        get => publishers;
        private set
        {
            publishers = value;
            StateHasChanged();
        }
    }

    [Inject]
    public IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel> WebManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Publishers = await WebManager.GetAllAsync();
    }
}
