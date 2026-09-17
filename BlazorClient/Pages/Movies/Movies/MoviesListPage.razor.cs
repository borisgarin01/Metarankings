using Domain.Movies;
using Domain.RequestsModels.Movies;
using Domain.RequestsModels.Movies.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;
using Domain.RequestsModels.Movies.MoviesStudios;
using Domain.ResponsesModels;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using WebManagers;

namespace BlazorClient.Pages.Movies.Movies;

public partial class MoviesListPage : ComponentBase
{
    private IEnumerable<MovieStudio> movieStudios = Enumerable.Empty<MovieStudio>();
    private IEnumerable<Movie> movies = Enumerable.Empty<Movie>();
    private IEnumerable<Domain.Movies.Genre> genres = Enumerable.Empty<Domain.Movies.Genre>();
    private PagedResponse<Movie> pagedResponse;
    private int currentPage = 1;
    private const int PageSize = 10;
    private bool isLoading = false;

    [SupplyParameterFromQuery] public int? Year { get; set; }
    [SupplyParameterFromQuery] public long? GenreId { get; set; }
    [SupplyParameterFromQuery] public long? MovieStudioId { get; set; }
    [SupplyParameterFromQuery] public long? MovieDirectorId { get; set; }
    [SupplyParameterFromQuery] public int? Page { get; set; }

    public IEnumerable<Movie> Movies
    {
        get => movies;
        set { movies = value; StateHasChanged(); }
    }

    public IEnumerable<MovieStudio> MovieStudios
    {
        get => movieStudios;
        set { movieStudios = value; StateHasChanged(); }
    }

    public IEnumerable<Domain.Movies.Genre> Genres
    {
        get => genres;
        set { genres = value; StateHasChanged(); }
    }

    public PagedResponse<Movie> PagedResponse
    {
        get => pagedResponse;
        set { pagedResponse = value; StateHasChanged(); }
    }

    [Inject] public IHttpClientFactory HttpClientFactory { get; set; } = default!;
    [Inject] public IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel> MovieStudiosWebManager { get; set; } = default!;
    [Inject] public IWebManager<Domain.Movies.Genre, AddMovieGenreModel, UpdateMovieGenreModel> GenresWebManager { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        isLoading = true;
        try
        {
            currentPage = Page ?? 1;

            var filter = new MovieFilterRequest
            {
                Skip = (currentPage - 1) * PageSize,
                Take = PageSize
            };

            if (GenreId.HasValue)
                filter.GenresIds = new[] { GenreId.Value };

            if (MovieStudioId.HasValue)
                filter.MoviesStudiosIds = new[] { MovieStudioId.Value };

            if (Year.HasValue)
                filter.Years = new[] { Year.Value };

            if (MovieDirectorId.HasValue)
                filter.MoviesDirectorsIds = new[] { MovieDirectorId.Value };

            HttpResponseMessage response = await HttpClientFactory
                .CreateClient("AuthorizedClient")
                .PostAsJsonAsync("/api/movies/byParameters", filter);

            if (response.IsSuccessStatusCode)
            {
                PagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<Movie>>();
                Movies = PagedResponse?.Items ?? Enumerable.Empty<Movie>();
            }
            else
            {
                Movies = Enumerable.Empty<Movie>();
                PagedResponse = null;
                Console.WriteLine($"Failed to load movies: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading movies: {ex.Message}");
            Movies = Enumerable.Empty<Movie>();
            PagedResponse = null;
        }
        finally
        {
            isLoading = false;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        Task<IEnumerable<MovieStudio>> studiosTask = MovieStudiosWebManager.GetAllAsync();
        Task<IEnumerable<Domain.Movies.Genre>> genresTask = GenresWebManager.GetAllAsync();

        await Task.WhenAll(studiosTask, genresTask);

        MovieStudios = studiosTask.Result;
        Genres = genresTask.Result;
    }

    private string BuildQueryString(int? page = null, int? year = null, long? genreId = null,
        long? movieStudioId = null, long? movieDirectorId = null)
    {
        var parameters = new List<string>();

        int? targetYear = year ?? Year;
        long? targetGenre = genreId ?? GenreId;
        long? targetStudio = movieStudioId ?? MovieStudioId;
        long? targetDirector = movieDirectorId ?? MovieDirectorId;
        int targetPage = page ?? currentPage;

        if (targetYear.HasValue)
            parameters.Add($"Year={targetYear.Value}");

        if (targetGenre.HasValue)
            parameters.Add($"GenreId={targetGenre}");

        if (targetStudio.HasValue)
            parameters.Add($"MovieStudioId={targetStudio}");

        if (targetDirector.HasValue)
            parameters.Add($"MovieDirectorId={targetDirector}");

        if (targetPage > 1)
            parameters.Add($"Page={targetPage}");

        return parameters.Any() ? $"?{string.Join("&", parameters)}" : "";
    }

    private string GetPageUrl(int page)
    {
        return $"/movies/best-movies{BuildQueryString(page)}";
    }
}