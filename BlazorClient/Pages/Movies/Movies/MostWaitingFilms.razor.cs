using Domain.Common.News;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;
using System.Globalization;
using WebManagers;
using WebManagers.Derived;
using WebManagers.Derived.Movies;

namespace BlazorClient.Pages.Movies.Movies;

public partial class MostWaitingFilms : ComponentBase
{
    private const int PageSize = 25;
    private const string BasePath = "/movies/most-waiting-movies";
    private static readonly CultureInfo RussianCulture = new CultureInfo("ru-RU");

    private bool isLoading = false;
    private int currentPage = 1;

    [Inject]
    public NewsWebManager NewsWebManager { get; set; } = default!;

    [Inject]
    public MoviesWebManager MoviesWebManager { get; set; } = default!;

    [Inject]
    public IWebManager<Genre, AddMovieGenreModel, UpdateMovieGenreModel> MoviesGenresWebManager { get; set; } = default!;

    [Inject]
    public TextTruncater TextTruncater { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "genreId")]
    public long? GenreId { get; set; }

    [SupplyParameterFromQuery(Name = "page")]
    public int? Page { get; set; }

    public IEnumerable<Movie> MostWaitingMovies { get; set; } = Enumerable.Empty<Movie>();
    public IEnumerable<Genre> MoviesGenres { get; set; } = Enumerable.Empty<Genre>();
    public IEnumerable<NewsItem> NewsItems { get; set; } = Enumerable.Empty<NewsItem>();

    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    protected override async Task OnInitializedAsync()
    {
        Task<IEnumerable<Genre>> genresTask = MoviesGenresWebManager.GetAllAsync();
        Task<IEnumerable<NewsItem>> newsItemsTask = NewsWebManager.GetAllAsync();

        try
        {
            await Task.WhenAll(genresTask, newsItemsTask);
            MoviesGenres = genresTask.Result ?? Enumerable.Empty<Genre>();
            NewsItems = newsItemsTask.Result ?? Enumerable.Empty<NewsItem>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load filters: {ex.Message}");
            MoviesGenres = Enumerable.Empty<Genre>();
            NewsItems = Enumerable.Empty<NewsItem>();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        isLoading = true;
        StateHasChanged();

        try
        {
            currentPage = Page ?? 1;
            if (currentPage < 1) currentPage = 1;

            long[]? genresIds = GenreId.HasValue ? new[] { GenreId.Value } : null;
            int offset = (currentPage - 1) * PageSize;

            Task<IEnumerable<Movie>> moviesTask = MoviesWebManager.GetMostWaitingAsync(offset, PageSize, genresIds);
            Task<int> countTask = MoviesWebManager.GetMostWaitingCountAsync(genresIds);

            await Task.WhenAll(moviesTask, countTask);

            MostWaitingMovies = moviesTask.Result ?? Enumerable.Empty<Movie>();
            TotalCount = countTask.Result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load most waiting movies: {ex.Message}");
            MostWaitingMovies = Enumerable.Empty<Movie>();
            TotalCount = 0;
        }
        finally
        {
            isLoading = false;
        }
    }

    private string BuildPageUrl(int? page = null, long? genreId = null)
    {
        List<string> parameters = new List<string>();

        long? targetGenre = genreId ?? GenreId;
        int targetPage = page ?? currentPage;

        if (targetGenre.HasValue)
            parameters.Add($"genreId={targetGenre.Value}");

        if (targetPage > 1)
            parameters.Add($"page={targetPage}");

        return parameters.Count > 0 ? $"{BasePath}?{string.Join("&", parameters)}" : BasePath;
    }

    private string GetPageUrl(int page)
    {
        return BuildPageUrl(page);
    }
}
