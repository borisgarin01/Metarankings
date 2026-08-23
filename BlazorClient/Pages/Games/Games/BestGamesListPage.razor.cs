using Domain.Games;
using Domain.RequestsModels;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Platforms;
using WebManagers;

namespace BlazorClient.Pages.Games.Games;

public partial class BestGamesListPage : ComponentBase
{
    private IEnumerable<Platform> platforms;
    private IEnumerable<Game> games;
    private IEnumerable<Genre> genres;

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

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> PlatformsWebManager { get; set; }

    [Inject]
    public IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel> GenresWebManager { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            // Всегда делаем запрос, даже если нет параметров
            GameFilterRequest filter = new GameFilterRequest
            {
                Skip = 0,
                Take = 10
            };

            // Добавляем параметры ТОЛЬКО если они есть
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

            // Всегда отправляем запрос
            HttpResponseMessage response = await HttpClientFactory.CreateClient("AuthorizedClient")
                .PostAsJsonAsync("/api/games/games/byParameters", filter);

            if (response.IsSuccessStatusCode)
            {
                Games = await response.Content.ReadFromJsonAsync<IEnumerable<Game>>();
            }
            else
            {
                Games = Enumerable.Empty<Game>();
                Console.WriteLine($"Failed to load games: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading games: {ex.Message}");
            Games = Enumerable.Empty<Game>();
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
}
