using Domain.Common.News;
using Domain.Games;
using WebManagers.Derived;
using WebManagers.Derived.Games;

namespace BlazorClient.Pages.News;

public partial class Details : ComponentBase
{
    [Parameter, EditorRequired]
    public long Id { get; set; }

    protected NewsItem? NewsItem { get; private set; }

    protected IEnumerable<NewsItem> NewsItems { get; private set; } = Enumerable.Empty<NewsItem>();

    protected IEnumerable<Game> SimilarGames { get; private set; } = Enumerable.Empty<Game>();

    [Inject]
    public NewsWebManager NewsWebManager { get; set; } = default!;

    [Inject]
    public GamesWebManager GamesWebManager { get; set; } = default!;

    [Inject]
    public TextTruncater TextTruncater { get; set; }

    private long _loadedId = -1;

    protected override async Task OnParametersSetAsync()
    {
        // Если Id не изменился — ничего не делаем
        if (_loadedId == Id)
        {
            return;
        }

        _loadedId = Id;
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            var newsItemTask = NewsWebManager.GetAsync(Id);
            var newsItemsTask = NewsWebManager.GetAllAsync();
            var similarGamesTask = GamesWebManager.GetLastAsync(0, 3);

            await Task.WhenAll(newsItemTask, newsItemsTask, similarGamesTask);

            NewsItem = await newsItemTask;
            NewsItems = await newsItemsTask ?? Enumerable.Empty<NewsItem>();
            SimilarGames = await similarGamesTask ?? Enumerable.Empty<Game>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Details] Failed to load news Id={Id}: {ex}");
            NewsItem = null;
            NewsItems = Enumerable.Empty<NewsItem>();
            SimilarGames = Enumerable.Empty<Game>();
        }
    }
}