using Domain.RequestsModels.Games.Platforms;
using WebManagers;
using Domain.Games;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Admin.Games.Platforms;

[Authorize(Policy = "Admin")]
public partial class ListPlatformsPage : ComponentBase
{
    public IEnumerable<Platform> Platforms { get; private set; }

    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> WebManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Platforms = await WebManager.GetAllAsync();
    }
}