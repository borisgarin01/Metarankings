using Domain.Games;
using Domain.Movies;
using WebManagers.Derived.Games;
using WebManagers.Derived.Movies;

namespace BlazorClient.Pages.Search;

public partial class SearchPage : ComponentBase
{
    private IEnumerable<Movie> movies;
    private IEnumerable<Game> games;
    private bool isSearching;
    private string? searchText;

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    [SupplyParameterFromQuery]
    public string? SearchText
    {
        get => searchText;
        set
        {
            if (searchText != value)
            {
                searchText = value;
                StateHasChanged();
            }
        }
    }

    public IEnumerable<Movie> Movies
    {
        get => movies;
        private set
        {
            movies = value;
            StateHasChanged();
        }
    }

    public IEnumerable<Game> Games
    {
        get => games;
        private set
        {
            games = value;
            StateHasChanged();
        }
    }

    [Inject]
    public GamesWebManager GamesWebManager { get; set; }

    [Inject]
    public MoviesWebManager MoviesWebManager { get; set; }

    public bool IsSearching
    {
        get => isSearching;
        set
        {
            isSearching = value;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        // Если есть SearchText из URL - выполняем поиск
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            await PerformSearch();
        }
        else
        {
            // Показываем пустые результаты или все элементы
            Games = Enumerable.Empty<Game>();
            Movies = Enumerable.Empty<Movie>();
        }
    }

    private async Task PerformSearch()
    {
        IsSearching = true;

        // Обновляем URL при поиске (НО без перезагрузки страницы)
        var currentUri = NavigationManager.Uri;
        var uriBuilder = new UriBuilder(currentUri);
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query["SearchText"] = SearchText;
        }
        else
        {
            query.Remove("SearchText");
        }

        uriBuilder.Query = query.ToString();
        var newUri = uriBuilder.Uri.PathAndQuery;

        // Обновляем URL без перезагрузки страницы
        if (NavigationManager.Uri != NavigationManager.BaseUri + newUri.TrimStart('/'))
        {
            NavigationManager.NavigateTo(newUri, false);
        }

        // Выполняем поиск
        Task<IEnumerable<Movie>> moviesSearchingTask;
        Task<IEnumerable<Game>> gamesSearchingTask;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            moviesSearchingTask = MoviesWebManager.SearchByName(SearchText);
            gamesSearchingTask = GamesWebManager.SearchByName(SearchText);
        }
        else
        {
            moviesSearchingTask = MoviesWebManager.GetAllAsync();
            gamesSearchingTask = GamesWebManager.GetAllAsync();
        }

        try
        {
            await Task.WhenAll(moviesSearchingTask, gamesSearchingTask);
            Movies = moviesSearchingTask.Result ?? Enumerable.Empty<Movie>();
            Games = gamesSearchingTask.Result ?? Enumerable.Empty<Game>();
        }
        catch (Exception ex)
        {
            // Обработка ошибок
            Movies = Enumerable.Empty<Movie>();
            Games = Enumerable.Empty<Game>();
            // Логирование ошибки
        }
        finally
        {
            IsSearching = false;
        }
    }

    private async void ClearSearch()
    {
        SearchText = string.Empty;
        Games = Enumerable.Empty<Game>();
        Movies = Enumerable.Empty<Movie>();

        // Очищаем URL
        NavigationManager.NavigateTo("/search", false);

        // Необязательно: выполнить поиск для отображения всех элементов
        // await PerformSearch();
    }
}
