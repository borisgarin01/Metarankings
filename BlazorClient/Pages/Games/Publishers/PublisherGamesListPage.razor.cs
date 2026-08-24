using Domain.Games;
using Domain.RequestsModels.Games.Developers;
using Domain.RequestsModels.Games.Platforms;
using Domain.RequestsModels.Games.Publishers;
using WebManagers;

namespace BlazorClient.Pages.Games.Publishers;

public partial class PublisherGamesListPage : ComponentBase
{
    private Publisher publisher;
    private IEnumerable<Platform> platforms;
    private IEnumerable<Developer> developers;

    [Parameter]
    public int PublisherId { get; set; }

    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> PlatformsWebManager { get; set; }

    [Inject]
    public IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel> PublishersWebManager { get; set; }

    [Inject]
    public IWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel> DevelopersWebManager { get; set; }

    public Publisher Publisher
    {
        get => publisher;
        set
        {
            publisher = value;
            StateHasChanged();
        }
    }
    public IEnumerable<Platform> Platforms
    {
        get => platforms;
        set
        {
            platforms = value;
            StateHasChanged();
        }
    }
    public IEnumerable<Developer> Developers
    {
        get => developers;
        set
        {
            developers = value;
            StateHasChanged();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        Task<Publisher> publisherGettingTask = PublishersWebManager.GetAsync(PublisherId);
        Task<IEnumerable<Platform>> platformsGettingTask = PlatformsWebManager.GetFirstAsync(0, 5);
        Task<IEnumerable<Developer>> developersGettingTask = DevelopersWebManager.GetFirstAsync(0, 5);

        await Task.WhenAll(publisherGettingTask, platformsGettingTask, developersGettingTask).ContinueWith(b =>
        {
            Publisher = publisherGettingTask.Result;
            Platforms = platformsGettingTask.Result;
            Developers = developersGettingTask.Result;
        });
    }
}