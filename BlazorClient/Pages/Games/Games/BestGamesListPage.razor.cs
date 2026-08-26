using Domain.Games;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Platforms;
using Domain.ResponsesModels;
using WebManagers;

namespace BlazorClient.Pages.Games.Games;

public partial class BestGamesListPage : ComponentBase
{
    private IEnumerable<Platform> platforms;
    private IEnumerable<Game> games;
    private IEnumerable<Genre> genres;
    private PagedResponse<Game> pagedResponse;
    private int currentPage = 1;
    private const int PageSize = 10;
    private bool isLoading = false;

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

    [SupplyParameterFromQuery]
    public long? LocalizationId { get; set; }

    [SupplyParameterFromQuery]
    public int? Page { get; set; }

    public IEnumerable<Game> Games
    {
        get => games;
        set
        {
            games = value;
            StateHasChanged();
        }
    }

    public IEnumerable<Platform> Platforms
    {
        get => platforms;
        set
        {
            platforms = value;
            StateHasChanged();
        }
    }

    public IEnumerable<Genre> Genres
    {
        get => genres;
        set
        {
            genres = value;
            StateHasChanged();
        }
    }

    public PagedResponse<Game> PagedResponse
    {
        get => pagedResponse;
        set
        {
            pagedResponse = value;
            StateHasChanged();
        }
    }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> PlatformsWebManager { get; set; }

    [Inject]
    public IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel> GenresWebManager { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        isLoading = true;
        try
        {
            currentPage = Page ?? 1;

            GameFilterRequest filter = new GameFilterRequest
            {
                Skip = (currentPage - 1) * PageSize,
                Take = PageSize
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

            if (LocalizationId.HasValue)
                filter.LocalizationIds = new[] { LocalizationId.Value };

            HttpResponseMessage response = await HttpClientFactory.CreateClient("AuthorizedClient")
                .PostAsJsonAsync("/api/games/games/byParameters", filter);

            if (response.IsSuccessStatusCode)
            {
                PagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<Game>>();
                Games = PagedResponse?.Items ?? Enumerable.Empty<Game>();
            }
            else
            {
                Games = Enumerable.Empty<Game>();
                PagedResponse = null;
                Console.WriteLine($"Failed to load games: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading games: {ex.Message}");
            Games = Enumerable.Empty<Game>();
            PagedResponse = null;
        }
        finally
        {
            isLoading = false;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        Task<IEnumerable<Platform>> platformsGettingTask = PlatformsWebManager.GetAllAsync();
        Task<IEnumerable<Genre>> gamesGenresGettingTask = GenresWebManager.GetAllAsync();

        await Task.WhenAll(platformsGettingTask, gamesGenresGettingTask).ContinueWith(b =>
        {
            Platforms = platformsGettingTask.Result;
            Genres = gamesGenresGettingTask.Result;
        });
    }

    private string BuildQueryString(int? page = null, int? year = null, long? genreId = null, long? platformId = null)
    {
        var parameters = new List<string>();

        int targetYear = year ?? Year ?? DateTime.Today.Year;
        long? targetGenre = genreId ?? GenreId;
        long? targetPlatform = platformId ?? PlatformId;
        int targetPage = page ?? currentPage;

        if (targetYear > 0)
            parameters.Add($"Year={targetYear}");

        if (targetGenre.HasValue)
            parameters.Add($"GenreId={targetGenre}");

        if (targetPlatform.HasValue)
            parameters.Add($"PlatformId={targetPlatform}");

        if (targetPage > 1)
            parameters.Add($"Page={targetPage}");

        return parameters.Any() ? $"?{string.Join("&", parameters)}" : "";
    }

    private string GetPageUrl(int page)
    {
        return $"/games/best-games{BuildQueryString(page)}";
    }
}