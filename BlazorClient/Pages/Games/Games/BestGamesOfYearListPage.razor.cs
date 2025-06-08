using Domain;
using Domain.Games;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.Json;

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


    public IEnumerable<GameModel> Games { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        var gamesGettingRequestModel = new GamesGettingRequestModel
        {
            DevelopersIds = DeveloperId.HasValue ? [DeveloperId.Value] : null,
            GenresIds = GenreId.HasValue ? [GenreId.Value] : null,
            PlatformsIds = PlatformId.HasValue ? [PlatformId.Value] : null,
            ReleasesYears = Year.HasValue ? [Year.Value] : null,
            PublishersIds = PublisherId.HasValue ? [PublisherId.Value] : null
        };

        var httpResponseMessage = await HttpClient.PostAsJsonAsync($"{HttpClient.BaseAddress}api/games/gamesByParameters", gamesGettingRequestModel);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            try
            {
                var games = await JsonSerializer.DeserializeAsync<IEnumerable<GameModel>>(await httpResponseMessage.Content.ReadAsStreamAsync());

                Games = games;
            }
            catch
            {
                Games = Enumerable.Empty<GameModel>();
            }
        }
    }
}
