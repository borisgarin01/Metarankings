using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Platforms;
using WebManagers;
using WebManagers.Derived.Games;

namespace BlazorClient.Pages.Games.Games;

public partial class MostWaitingGames : ComponentBase
{
    private const int PageSize = 25;
    private bool isLoading = false;

    [Inject]
    public GamesWebManager GamesWebManager { get; set; } = default!;

    [Inject]
    public IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel> GamesGenresWebManager { get; set; } = default!;

    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> GamesPlatformsWebManager { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "genreId")]
    public long? GenreId { get; set; }

    [SupplyParameterFromQuery(Name = "platformId")]
    public long? PlatformId { get; set; }

    [SupplyParameterFromQuery(Name = "page")]
    public int? Page { get; set; }

    public IEnumerable<Game> FutureGames { get; set; } = Enumerable.Empty<Game>();
    public IEnumerable<Genre> GamesGenres { get; set; } = Enumerable.Empty<Genre>();
    public IEnumerable<Platform> GamesPlatforms { get; set; } = Enumerable.Empty<Platform>();

    public int TotalCount { get; set; }
    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling((double)TotalCount / PageSize)
        : 1;

    private int currentPage = 1;

    protected override async Task OnInitializedAsync()
    {
        Task<IEnumerable<Genre>> genresTask = GamesGenresWebManager.GetAllAsync();
        Task<IEnumerable<Platform>> platformsTask = GamesPlatformsWebManager.GetAllAsync();

        try
        {
            await Task.WhenAll(genresTask, platformsTask);
            GamesGenres = genresTask.Result ?? Enumerable.Empty<Genre>();
            GamesPlatforms = platformsTask.Result ?? Enumerable.Empty<Platform>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load filters: {ex.Message}");
            GamesGenres = Enumerable.Empty<Genre>();
            GamesPlatforms = Enumerable.Empty<Platform>();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        isLoading = true;
        try
        {
            currentPage = Page ?? 1;
            if (currentPage < 1) currentPage = 1;

            long[]? genresIds = GenreId.HasValue ? new[] { GenreId.Value } : null;
            long[]? platformsIds = PlatformId.HasValue ? new[] { PlatformId.Value } : null;

            long offset = (currentPage - 1) * (long)PageSize;

            Task<IEnumerable<Game>> gamesTask = GamesWebManager.GetNearestAsync(
                offset, PageSize, genresIds, platformsIds);

            Task<int> countTask = GamesWebManager.GetNearestCountAsync(
                genresIds, platformsIds);

            await Task.WhenAll(gamesTask, countTask);

            FutureGames = gamesTask.Result ?? Enumerable.Empty<Game>();
            TotalCount = countTask.Result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load future games: {ex.Message}");
            FutureGames = Enumerable.Empty<Game>();
            TotalCount = 0;
        }
        finally
        {
            isLoading = false;
        }
    }

    // --- URL builder, по образу BestGamesListPage ---

    private string BuildQueryString(int? page = null, long? genreId = null, long? platformId = null)
    {
        var parameters = new List<string>();

        long? targetGenre = genreId ?? GenreId;
        long? targetPlatform = platformId ?? PlatformId;
        int targetPage = page ?? currentPage;

        if (targetGenre.HasValue)
            parameters.Add($"genreId={targetGenre.Value}");

        if (targetPlatform.HasValue)
            parameters.Add($"platformId={targetPlatform.Value}");

        if (targetPage > 1)
            parameters.Add($"page={targetPage}");

        return parameters.Count > 0 ? $"?{string.Join("&", parameters)}" : "";
    }

    private string GetPageUrl(int page)
    {
        return $"/games/most-waiting-games{BuildQueryString(page)}";
    }
}