using Domain.Common.News;
using Domain.Reviews;
using WebManagers.Derived;

namespace BlazorClient.Pages;

public partial class UserReviews : ComponentBase
{
    private IEnumerable<GameReview> gamesReviews;
    private IEnumerable<MovieViewerReview> movieViewersReviews;
    private IEnumerable<NewsItem> news;

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    [Inject]
    public NewsWebManager NewsWebManager { get; set; }

    [Inject]
    public TextTruncater TextTruncater { get; set; }

    [SupplyParameterFromQuery]
    public int PageSize { get; set; } = 5;

    [SupplyParameterFromQuery]
    public int PageNumber { get; set; } = 1;

    public IEnumerable<GameReview> GamesReviews
    {
        get => gamesReviews;
        set
        {
            gamesReviews = value;
            StateHasChanged();
        }
    }
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
        // защита от мусорных значений из query string
        if (PageNumber < 1) PageNumber = 1;
        if (PageSize < 1) PageSize = 5;
        // опционально — верхняя граница
        if (PageSize > 100) PageSize = 100;

        var httpClient = HttpClientFactory.CreateClient("AuthorizedClient");

        try
        {
            var gamesTask = httpClient.GetFromJsonAsync<IEnumerable<GameReview>>(
                $"/api/Games/GamesGamersReviews/{(PageNumber - 1) * PageSize}/{PageSize}");

            var moviesTask = httpClient.GetFromJsonAsync<IEnumerable<MovieViewerReview>>(
                $"/api/movies/moviesViewersReviews/{(PageNumber - 1) * PageSize}/{PageSize}");

            var newsTask = NewsWebManager.GetFirstAsync(0, 4);

            await Task.WhenAll(gamesTask, moviesTask, newsTask);

            GamesReviews = await gamesTask ?? Enumerable.Empty<GameReview>();
            MovieViewersReviews = await moviesTask ?? Enumerable.Empty<MovieViewerReview>();
            News = await newsTask ?? Enumerable.Empty<NewsItem>();
        }
        catch (Exception ex)
        {
            // логируем, но не роняем страницу
            GamesReviews ??= Enumerable.Empty<GameReview>();
            MovieViewersReviews ??= Enumerable.Empty<MovieViewerReview>();
            News ??= Enumerable.Empty<NewsItem>();
        }
    }
}
