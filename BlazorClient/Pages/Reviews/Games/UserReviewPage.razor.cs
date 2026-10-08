using Domain.Common.News;
using Domain.Reviews;
using WebManagers.Derived;

namespace BlazorClient.Pages.Reviews.Games;

public partial class UserReviewPage : CancellableComponentBase
{
    private GameReview gameGamerReview;
    private IEnumerable<NewsItem> news;

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    [Parameter, EditorRequired]
    public long ReviewId { get; set; }

    [Inject]
    public NewsWebManager NewsWebManager { get; set; }

    [Inject]
    public TextTruncater TextTruncater { get; set; }

    public GameReview GameGamerReview
    {
        get => gameGamerReview;
        set
        {
            gameGamerReview = value;
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

        Task<GameReview> reviewGettingTask = httpClient.GetFromJsonAsync<GameReview>(
                $"/api/games/GamesGamersReviews/{ReviewId}", DisposalToken);

        Task<IEnumerable<NewsItem>> newsGettingTask = NewsWebManager.GetFirstAsync(0, 4, DisposalToken);

        await Task.WhenAll(reviewGettingTask, newsGettingTask);

        GameGamerReview = reviewGettingTask.Result;
        News = newsGettingTask.Result;
    }
}
