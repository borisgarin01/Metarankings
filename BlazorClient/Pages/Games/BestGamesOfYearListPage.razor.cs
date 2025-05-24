using Domain;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http.Json;

namespace BlazorClient.Pages.Games;

public partial class BestGamesOfYearListPage : ComponentBase
{
    [Parameter]
    public int? Year { get; set; }

    [SupplyParameterFromQuery]
    public int? GenreId { get; set; }

    public IEnumerable<GameModel> Games { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (!Year.HasValue)
        {
            if (!GenreId.HasValue)
                Games = await HttpClient.GetFromJsonAsync<IEnumerable<GameModel>>($"/api/Games/5/1");
            else
                Games = await HttpClient.GetFromJsonAsync<IEnumerable<GameModel>>($"/api/Games/5/1");
        }
        else
        {
            Games = await HttpClient.GetFromJsonAsync<IEnumerable<GameModel>>($"/api/Games/Year/{Year.Value}");
        }
    }
}
