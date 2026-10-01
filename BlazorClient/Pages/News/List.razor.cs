using Domain.Common.News;
using Domain.Games;
using Domain.RequestsModels.Games.Platforms;
using WebManagers;
using WebManagers.Derived;
using WebManagers.Derived.Games;

namespace BlazorClient.Pages.News;

public partial class List
{
    private const int PageSize = 15;

    [Inject]
    public NewsWebManager NewsWebManager { get; set; } = default!;

    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> PlatformsWebManager { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    protected List<NewsItem> News { get; } = new();

    protected List<Platform> Platforms { get; } = new();

    protected int CurrentPage { get; private set; } = 1;

    protected bool IsLoading { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadPlatformsAsync();
        await LoadPageAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await RenderAdsAsync();
        }
    }

    protected async Task LoadNextPageAsync()
    {
        if (IsLoading)
        {
            return;
        }

        CurrentPage++;
        await LoadPageAsync();
    }

    private async Task LoadPageAsync()
    {
        IsLoading = true;

        try
        {
            long offset = (CurrentPage - 1) * PageSize;
            var items = await NewsWebManager.GetFirstAsync(offset, PageSize);

            News.Clear();
            News.AddRange(items ?? Enumerable.Empty<NewsItem>());
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadPlatformsAsync()
    {
        var platforms = await PlatformsWebManager.GetAllAsync();
        Platforms.Clear();
        Platforms.AddRange(platforms ?? Enumerable.Empty<Platform>());
    }

    private async Task RenderAdsAsync()
    {
        await JSRuntime.InvokeVoidAsync("renderYandexAds", "R-A-201169-5");
        await JSRuntime.InvokeVoidAsync("renderYandexAds", "R-A-201169-26");
    }

    private static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
    }
}