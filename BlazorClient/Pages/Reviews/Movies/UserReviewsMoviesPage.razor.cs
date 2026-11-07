using Domain.Common.News;
using Domain.Reviews;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using WebManagers.Derived;

namespace BlazorClient.Pages.Reviews.Movies;

public partial class UserReviewsMoviesPage : ComponentBase
{
    private IEnumerable<MovieViewerReview> movieViewersReviews;
    private IEnumerable<NewsItem> news;

    [Inject] public IHttpClientFactory HttpClientFactory { get; set; }
    [Inject] public NewsWebManager NewsWebManager { get; set; }
    [Inject] public TextTruncater TextTruncater { get; set; }

    [SupplyParameterFromQuery]
    public int PageSize { get; set; } = 5;

    [SupplyParameterFromQuery]
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; private set; } = 1;

    public IEnumerable<MovieViewerReview> MovieViewersReviews
    {
        get => movieViewersReviews;
        set
        {
            movieViewersReviews = value;
            StateHasChanged();
        }
    }
    public IEnumerable<NewsItem> News
    {
        get => news;
        set
        {
            news = value;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (PageNumber < 1) PageNumber = 1;
        if (PageSize < 1) PageSize = 5;
        if (PageSize > 100) PageSize = 100;

        var httpClient = HttpClientFactory.CreateClient("AuthorizedClient");

        try
        {
            var moviesTask = httpClient.GetFromJsonAsync<IEnumerable<MovieViewerReview>>(
                $"/api/movies/moviesViewersReviews/{(PageNumber - 1) * PageSize}/{PageSize}");

            var newsTask = NewsWebManager.GetFirstAsync(0, 4);

            await Task.WhenAll(moviesTask, newsTask);

            MovieViewersReviews = await moviesTask ?? Enumerable.Empty<MovieViewerReview>();
            News = await newsTask ?? Enumerable.Empty<NewsItem>();

            TotalPages = await GetTotalPagesAsync(httpClient);
        }
        catch
        {
            MovieViewersReviews ??= Enumerable.Empty<MovieViewerReview>();
            News ??= Enumerable.Empty<NewsItem>();
            TotalPages = 1;
        }
    }

    private async Task<int> GetTotalPagesAsync(HttpClient httpClient)
    {
        try
        {
            var total = await httpClient.GetFromJsonAsync<int>(
                $"/api/movies/moviesViewersReviews/Count");
            return Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        }
        catch
        {
            return MovieViewersReviews.Count() == PageSize
                ? PageNumber + 1
                : PageNumber;
        }
    }
}