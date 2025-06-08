using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Domain.Games;

namespace BlazorClient.Pages.Games.Genres;

public partial class GenreGamesListPage : ComponentBase
{
    [Parameter]
    public long GenreId { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    public Genre Genre { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Genre = await HttpClient.GetFromJsonAsync<Genre>($"/api/Genres/{GenreId}");
    }
}
