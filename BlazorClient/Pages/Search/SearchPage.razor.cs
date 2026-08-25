using Domain.Games;
using Domain.Movies;
using WebManagers.Derived.Games;
using WebManagers.Derived.Movies;

namespace BlazorClient.Pages.Search;

public partial class SearchPage : ComponentBase
{
    private IEnumerable<Movie> movies = Enumerable.Empty<Movie>();
    private IEnumerable<Game> games = Enumerable.Empty<Game>();
    private bool isSearching;
    private string? searchText;
    private bool isInitialized;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public GamesWebManager GamesWebManager { get; set; } = default!;

    [Inject]
    public MoviesWebManager MoviesWebManager { get; set; } = default!;

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

    public bool IsSearching
    {
        get => isSearching;
        private set
        {
            isSearching = value;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        isInitialized = true;
        await PerformSearch();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (isInitialized)
        {
            await PerformSearch();
        }
    }

    private async Task PerformSearch()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Movies = Enumerable.Empty<Movie>();
            Games = Enumerable.Empty<Game>();
            return;
        }

        if (IsSearching)
            return;

        IsSearching = true;

        try
        {
            Task<IEnumerable<Movie>> moviesTask = MoviesWebManager.SearchByName(SearchText);
            Task<IEnumerable<Game>> gamesTask = GamesWebManager.SearchByName(SearchText);

            await Task.WhenAll(moviesTask, gamesTask);

            Movies = moviesTask.Result ?? Enumerable.Empty<Movie>();
            Games = gamesTask.Result ?? Enumerable.Empty<Game>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Search error: {ex.Message}");
            Movies = Enumerable.Empty<Movie>();
            Games = Enumerable.Empty<Game>();
        }
        finally
        {
            IsSearching = false;
        }
    }

    private async Task ClearSearch()
    {
        SearchText = string.Empty;
        Movies = Enumerable.Empty<Movie>();
        Games = Enumerable.Empty<Game>();

        NavigationManager.NavigateTo("/search", false);

        await Task.Delay(100);
    }
}