using Domain.Games;
using System;

namespace BlazorClient.Pages.Games.Games;

public partial class BestGamesOfYearListPage : ComponentBase
{
    [SupplyParameterFromQuery]
    public int? Year { get; set; }

    [SupplyParameterFromQuery]
    public int? GenreId { get; set; }

    [SupplyParameterFromQuery]
    public int? PlatformId { get; set; }

    [SupplyParameterFromQuery]
    public int? DeveloperId { get; set; }

    [SupplyParameterFromQuery]
    public int? PublisherId { get; set; }

    public IEnumerable<Game> Games { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Games = await HttpClient.GetFromJsonAsync<IEnumerable<Game>>($"{HttpClient.BaseAddress}api/games/games/year/{Year}");
    }
}
