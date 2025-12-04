using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Collections;

public partial class ManageCollectionPage : ComponentBase
{
    [Inject]
    public IWebManager<GameCollection, AddGameCollectionModel, UpdateGameCollectionModel> GamesCollectionsWebManager { get; set; }

    [Inject]
    public IWebManager<GameCollectionItem, AddGameCollectionItemModel, UpdateGameCollectionItemModel> GamesCollectionsItemsWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private GameCollection gameCollection;

    [Parameter, EditorRequired]
    public long Id { get; set; }

    public GameCollection GameCollection
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
