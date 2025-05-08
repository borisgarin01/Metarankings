using Microsoft.AspNetCore.Components;
using Domain;
using System.Net.Http.Json;

namespace BlazorClient.Pages.Platforms;

public partial class PlatformGamesListPage : ComponentBase
{
    [Parameter]
    public string PlatformName { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    public IEnumerable<GameModel> GamesOfPlatform { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        GamesOfPlatform = await HttpClient.GetFromJsonAsync<IEnumerable<GameModel>>($"/api/Games/Platforms/{PlatformName}");
    }
}
