using Domain;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages.Developers;

public partial class DeveloperGamesListPage : ComponentBase
{
    [Parameter]
    public string DeveloperName { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    public IEnumerable<GameModel> GamesOfDeveloper { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        GamesOfDeveloper = await HttpClient.GetFromJsonAsync<IEnumerable<GameModel>>($"/api/Games/developers/{DeveloperName}");
    }
}
