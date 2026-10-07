using Domain.RequestsModels.Games.Developers;
using WebManagers;
using Domain.Games;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Admin.Games.Developers;

[Authorize(Policy = "Admin")]
public partial class ListDevelopersPage : ComponentBase
{
    public IEnumerable<Developer> Developers { get; set; }

    [Inject]
    public IWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel> WebManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Developers = await WebManager.GetAllAsync();
    }
}