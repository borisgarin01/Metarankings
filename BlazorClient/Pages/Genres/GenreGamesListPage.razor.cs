using Microsoft.AspNetCore.Components;
using Domain;
using System.Net.Http.Json;

namespace BlazorClient.Pages.Genres;

public partial class GenreGamesListPage : ComponentBase
{
    [Parameter]
    public string GenreName { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    public IEnumerable<GameModel> GamesOfGenre { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        GamesOfGenre = await HttpClient.GetFromJsonAsync<IEnumerable<GameModel>>($"/api/Games/genres/{GenreName}");
    }
}
