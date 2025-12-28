using Domain.Games;
using Domain.RequestsModels;
using System;

namespace BlazorClient.Pages.Games.Games;

public partial class BestGamesOfYearListPage : ComponentBase
{
    [SupplyParameterFromQuery]
    public int? Year { get; set; }

    [SupplyParameterFromQuery]
    public long? GenreId { get; set; }

    [SupplyParameterFromQuery]
    public long? PlatformId { get; set; }

    [SupplyParameterFromQuery]
    public long? DeveloperId { get; set; }

    [SupplyParameterFromQuery]
    public long? PublisherId { get; set; }

    public IEnumerable<Game> Games { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            if (Year.HasValue || PlatformId.HasValue || GenreId.HasValue || DeveloperId.HasValue || PublisherId.HasValue)
            {
                var filter = new GameFilterRequest
                {
                    Skip = 0,
                    Take = 10
                };

                if (GenreId.HasValue)
                    filter.GenresIds = new[] { GenreId.Value };

                if (PlatformId.HasValue)
                    filter.PlatformsIds = new[] { PlatformId.Value };

                if (Year.HasValue)
                    filter.Years = new[] { Year.Value };

                if (DeveloperId.HasValue)
                    filter.DevelopersIds = new[] { DeveloperId.Value };

                if (PublisherId.HasValue)
                    filter.PublishersIds = new[] { PublisherId.Value };

                // Using HttpClient.PostAsJsonAsync for POST or custom query string building for GET
                HttpResponseMessage response = await HttpClient.PostAsJsonAsync<GameFilterRequest>(
                    $@"{HttpClient.BaseAddress}api/games/games/byParameters",
                    filter
                );

                if (response.IsSuccessStatusCode)
                {
                    Games = await response.Content.ReadFromJsonAsync<IEnumerable<Game>>();
                }
                else
                {
                    Console.WriteLine($"Failed to load games: {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading games: {ex.Message}");
        }
    }
}
