using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using WebManagers;
using WebManagers.Derived.Games;

namespace BlazorClient.Pages.Games.Games;

public partial class MostWaitingGames : ComponentBase
{
    [Inject]
    public GamesWebManager GamesWebManager { get; set; } = default!;

    [Inject]
    public IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel> GamesGenresWebManager { get; set; } = default!;

    [Parameter]
    public int PageSize { get; set; } = 5;

    [Parameter]
    public int PageNumber { get; set; } = 1;

    [SupplyParameterFromQuery(Name = "platform")]
    public long? PlatformId { get; set; }

    [SupplyParameterFromQuery(Name = "genre")]
    public long? GenreId { get; set; }

    public IEnumerable<Game> FutureGames { get; set; } = Enumerable.Empty<Game>();

    public IEnumerable<Genre> GamesGenres { get; set; } = Enumerable.Empty<Genre>();

    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    protected override async Task OnParametersSetAsync()
    {
        if (PageNumber < 1)
            PageNumber = 1;
        if (PageSize < 1)
            PageSize = 5;

        var genresIds = GenreId.HasValue ? new[] { GenreId.Value } : null;
        var platformsIds = PlatformId.HasValue ? new[] { PlatformId.Value } : null;

        long offset = (PageNumber - 1) * PageSize;

        // Параллельно получаем игры и общее количество
        Task<IEnumerable<Game>> gamesTask = GamesWebManager.GetNearestAsync(
            offset, PageSize, genresIds, platformsIds);

        Task<int> countTask = GamesWebManager.GetNearestCountAsync(
            genresIds, platformsIds);

        Task<IEnumerable<Genre>> gamesGenresTask = GamesGenresWebManager.GetAllAsync();

        await Task.WhenAll(gamesTask, countTask, gamesGenresTask);

        FutureGames = gamesTask.Result;
        TotalCount = countTask.Result;
        GamesGenres = gamesGenresTask.Result;
    }
}
