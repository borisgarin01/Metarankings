
using Domain.Games;
using Domain.RequestsModels.Games;

namespace BlazorClient.Pages.Games.Admin.Games;

public partial class AddGamePage : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        Task<IEnumerable<Developer>> developersGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Developer>>("/api/Developers");
        Task<IEnumerable<Genre>> genresGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Genre>>("/api/Genres");
        Task<IEnumerable<Localization>> localizationsGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Localization>>("/api/Localizations");
        Task<IEnumerable<Platform>> platformsGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Platform>>("/api/Platforms");
        Task<IEnumerable<Publisher>> publishersGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Publisher>>("/api/Publishers");

        await Task.WhenAll(developersGettingTask, genresGettingTask, localizationsGettingTask, platformsGettingTask, publishersGettingTask);

        DevelopersToSelectFrom = developersGettingTask.Result;
        GenresToSelectFrom = genresGettingTask.Result;
        LocalizationsToSelectFrom = localizationsGettingTask.Result;
        PlatformsToSelectFrom = platformsGettingTask.Result;
        PublishersToSelectFrom = publishersGettingTask.Result;
    }

    [Inject]
    public HttpClient HttpClient { get; private set; }

    [Inject]
    public IJSRuntime JSRuntime { get; private set; }

    [Inject]
    public NavigationManager NavigationManager { get; private set; }

    public string Name { get; set; }
    public string Image { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Description { get; set; }
    public string Trailer { get; set; }

    public IEnumerable<Developer> DevelopersToSelectFrom { get; private set; }
    public IEnumerable<Genre> GenresToSelectFrom { get; private set; }
    public IEnumerable<Localization> LocalizationsToSelectFrom { get; private set; }
    public IEnumerable<Platform> PlatformsToSelectFrom { get; private set; }
    public IEnumerable<Publisher> PublishersToSelectFrom { get; private set; }

    public List<long> SelectedDevelopersIds { get; private set; } = new List<long>();
    public List<long> SelectedGenresIds { get; private set; } = new List<long>();
    public long SelectedLocalizationId { get; private set; }
    public List<long> SelectedPlatformsIds { get; private set; } = new List<long>();
    public long SelectedPublisherId { get; private set; }

    public async Task AddGameAsync()
    {
        if (GameModelToAddConfigured())
        {
            var addGameModel = new AddGameModel(Name, Image, SelectedDevelopersIds, SelectedPublisherId, SelectedGenresIds, SelectedLocalizationId, ReleaseDate, Description, Trailer, SelectedPlatformsIds);

            HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync<AddGameModel>("/api/games", addGameModel);

            if (httpResponseMessage != null)
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    NavigationManager.NavigateTo("/admin/games/list-games");
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
                }
            }
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("alert", "Please, fill all required fields");
        }
    }

    private bool GameModelToAddConfigured()
    {
        return !SelectedDevelopersIds.Contains(-1) && !SelectedGenresIds.Contains(-1) && SelectedLocalizationId != -1 && !SelectedPlatformsIds.Contains(-1) && SelectedPublisherId != -1;
    }

    private async Task SelectDeveloper(ChangeEventArgs e)
    {
        SelectedDevelopersIds = ((string[])e.Value)
            .Select(idString => long.Parse(idString))
            .ToList();
    }

    private async Task SelectGenre(ChangeEventArgs e)
    {
        SelectedGenresIds = ((string[])e.Value)
        .Select(idString => long.Parse(idString))
            .ToList();
    }

    private async Task SelectPublisher(ChangeEventArgs e)
    {
        SelectedPublisherId = long.Parse((string)e.Value);
    }

    private async Task SelectLocalization(ChangeEventArgs e)
    {
        SelectedLocalizationId = long.Parse((string)e.Value);
    }

    private async Task SelectPlatform(ChangeEventArgs e)
    {
        SelectedPlatformsIds = ((string[])e.Value)
        .Select(idString => long.Parse(idString))
            .ToList();
    }
}
