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
        var gamesGettingRequestModel = new GamesGettingRequestModel(
            Year.HasValue ? [Year.Value] : null,
            DeveloperId.HasValue ? [DeveloperId.Value] : null,
            GenreId.HasValue ? [GenreId.Value] : null,
            PublisherId.HasValue ? [PublisherId.Value] : null,
            PlatformId.HasValue ? [PlatformId.Value] : null);

        var httpResponseMessage = await HttpClient.PostAsJsonAsync($"{HttpClient.BaseAddress}api/games/gamesByParameters", gamesGettingRequestModel);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            try
            {
                var games = await JsonSerializer.DeserializeAsync<IEnumerable<Game>>(await httpResponseMessage.Content.ReadAsStreamAsync());

                Games = games;
            }
            catch
            {
                Games = Enumerable.Empty<Game>();
            }
        }
    }
}
