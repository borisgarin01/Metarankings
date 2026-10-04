using Domain.Common.News;
using Domain.Reviews;
using WebManagers.Derived;

namespace BlazorClient.Pages.Reviews.Movies;

public partial class UserReviewPage : ComponentBase
{
    private MovieViewerReview movieViewerReview;
    private IEnumerable<NewsItem> news;

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    [Parameter, EditorRequired]
    public long ReviewId { get; set; }

    [Inject]
    public NewsWebManager NewsWebManager { get; set; }

    [Inject]
    public TextTruncater TextTruncater { get; set; }

    public MovieViewerReview MovieViewerReview
    {
        get => movieViewerReview;
        set
        {
            movieViewerReview = value;
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
        var httpClient = HttpClientFactory.CreateClient("AuthorizedClient");

        Task<MovieViewerReview> reviewGettingTask = httpClient.GetFromJsonAsync<MovieViewerReview>(
                $"/api/Movies/MoviesViewersReviews/{ReviewId}");

        Task<IEnumerable<NewsItem>> newsGettingTask = NewsWebManager.GetFirstAsync(0, 4);

        await Task.WhenAll(reviewGettingTask, newsGettingTask);

        MovieViewerReview = reviewGettingTask.Result;
        News = newsGettingTask.Result;
    }
}
