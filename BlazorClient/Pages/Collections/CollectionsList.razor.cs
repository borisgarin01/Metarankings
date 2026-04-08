
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using WebManagers;
using WebManagers.Derived.Games;

namespace BlazorClient.Pages.Collections;

public partial class CollectionsList : ComponentBase
{
    private IEnumerable<GameCollection> gamesCollections;

    [Inject]
    public IWebManager<GameCollection, AddGameCollectionModel, UpdateGameCollectionModel> GamesCollectionsWebManager { get; set; }

    public IEnumerable<GameCollection> GamesCollections
    {
        get => gamesCollections;
        set
        {
            gamesCollections = value;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        GamesCollections = await GamesCollectionsWebManager.GetAllAsync();
    }

    public async Task DeleteCollectionAsync(long id)
    {
        await GamesCollectionsWebManager.DeleteAsync(id);
        GamesCollections = await GamesCollectionsWebManager.GetAllAsync();
    }
}
