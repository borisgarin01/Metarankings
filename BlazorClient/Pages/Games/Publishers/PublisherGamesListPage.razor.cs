using Domain.Games;
using Domain.RequestsModels.Games.Platforms;
using Domain.RequestsModels.Games.Publishers;
using WebManagers;

namespace BlazorClient.Pages.Games.Publishers;

public partial class PublisherGamesListPage : ComponentBase
{
    [Parameter]
    public int PublisherId { get; set; }

    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> PlatformsWebManager { get; set; }

    [Inject]
    public IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel> PublishersWebManager { get; set; }

    public Publisher Publisher { get; set; }
    public IEnumerable<Platform> Platforms { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Task<Publisher> publisherGettingTask = PublishersWebManager.GetAsync(PublisherId);
        Task<IEnumerable<Platform>> platformsGettingTask = PlatformsWebManager.GetFirstAsync(0, 5);

        await Task.WhenAll(publisherGettingTask, platformsGettingTask).ContinueWith(b =>
        {
            Publisher = publisherGettingTask.Result;
            Platforms = platformsGettingTask.Result;
        });
    }
}