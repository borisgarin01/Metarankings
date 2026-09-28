using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Platforms;
using Domain.ResponsesModels;
using WebManagers;

namespace BlazorClient.Pages.Games.Games;

public partial class GamesReleasesDatesPage : ComponentBase
{
    private IEnumerable<Platform> platforms;
    private IEnumerable<Game> games;
    private IEnumerable<Genre> genres;
    private PagedResponse<Game> pagedResponse;
    private int currentPage = 1;
    private const int PageSize = 10;
    private bool isLoading = false;

    [SupplyParameterFromQuery]
    public long? GenreId { get; set; }

    [SupplyParameterFromQuery]
    public long? PlatformId { get; set; }

    [SupplyParameterFromQuery]
    public int? PageNumber { get; set; }

    public IEnumerable<Game> FutureGames
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
            currentPage = PageNumber ?? 1;

            short offset = (short)((currentPage - 1) * PageSize);
            short limit = PageSize;

            // Формируем query string
            var queryParams = new List<string>();

            if (GenreId.HasValue)
                queryParams.Add($"genresIds={GenreId.Value}");

            if (PlatformId.HasValue)
                queryParams.Add($"platformsIds={PlatformId.Value}");

            string queryString = queryParams.Any()
                ? $"?{string.Join("&", queryParams)}"
                : "";

            // Загружаем игры
            HttpResponseMessage gamesResponse = await HttpClientFactory
                .CreateClient("AuthorizedClient")
                .GetAsync($"/api/games/games/games-releases-dates/{offset}/{limit}{queryString}");

            // Загружаем общее количество для пагинации
            HttpResponseMessage countResponse = await HttpClientFactory
                .CreateClient("AuthorizedClient")
                .GetAsync($"/api/games/games/games-releases-dates/count{queryString}");

            if (gamesResponse.IsSuccessStatusCode)
            {
                var gamesList = await gamesResponse.Content.ReadFromJsonAsync<IEnumerable<Game>>();
                FutureGames = gamesList ?? Enumerable.Empty<Game>();

                if (countResponse.IsSuccessStatusCode)
                {
                    int totalCount = await countResponse.Content.ReadFromJsonAsync<int>();

                    PagedResponse = new PagedResponse<Game>
                    {
                        Items = FutureGames,
                        Page = currentPage,
                        PageSize = PageSize,
                        TotalCount = totalCount
                    };
                }
            }
            else
            {
                FutureGames = Enumerable.Empty<Game>();
                PagedResponse = null;
                Console.WriteLine($"Failed to load games: {gamesResponse.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading games: {ex.Message}");
            FutureGames = Enumerable.Empty<Game>();
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

    private string BuildQueryString(int? page = null, long? genreId = null, long? platformId = null)
    {
        var parameters = new List<string>();

        long? targetGenre = genreId ?? GenreId;
        long? targetPlatform = platformId ?? PlatformId;
        int targetPage = page ?? currentPage;

        if (targetGenre.HasValue)
            parameters.Add($"GenreId={targetGenre}");

        if (targetPlatform.HasValue)
            parameters.Add($"PlatformId={targetPlatform}");

        if (targetPage > 1)
            parameters.Add($"PageNumber={targetPage}");

        parameters.Add($"PageSize={PageSize}");

        return parameters.Any() ? $"?{string.Join("&", parameters)}" : "";
    }

    private string GetPageUrl(int page)
    {
        return $"/games/releases-dates{BuildQueryString(page)}";
    }
}