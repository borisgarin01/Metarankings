using Domain;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages.Publishers;

public partial class PublisherGamesListPage : ComponentBase
{
    [Parameter]
    public string PublisherName { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    public IEnumerable<GameModel> GamesOfPublisher { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        GamesOfPublisher = await HttpClient.GetFromJsonAsync<IEnumerable<GameModel>>($"/api/Games/Publishers/{PublisherName}");
    }
}