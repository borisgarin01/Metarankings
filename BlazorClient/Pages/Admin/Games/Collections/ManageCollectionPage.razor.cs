using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Collections;

public partial class ManageCollectionPage : ComponentBase
{
    [Inject]
    public IWebManager<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel> GamesCollectionsWebManager { get; set; }

    [Inject]
    public IWebManager<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel> GamesCollectionsItemsWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    private GamesCollection gameCollection;

    [Parameter, EditorRequired]
    public long Id { get; set; }

    public GamesCollection GameCollection
    {
        get => gameCollection;
        private set
        {
            gameCollection = value;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        GameCollection = await GamesCollectionsWebManager.GetAsync(Id);
    }

    public async Task DeleteGameCollectionAsync(long id)
    {
        await GamesCollectionsWebManager.DeleteAsync(id);
        NavigationManager.NavigateTo("/admin");
    }

    public async Task DeleteGameFromCollectionAsync(long id)
    {
        await GamesCollectionsItemsWebManager.DeleteAsync(id);
        GameCollection = await GamesCollectionsWebManager.GetAsync(GameCollection.Id);
    }
}
