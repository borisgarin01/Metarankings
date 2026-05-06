using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using WebManagers;

namespace BlazorClient.Pages.Games.Collections;

public partial class CollectionsList : ComponentBase
{
    private IEnumerable<GamesCollection> gamesCollections;

    [Inject]
    public IWebManager<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel> GamesCollectionsWebManager { get; set; }

    public IEnumerable<GamesCollection> GamesCollections
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
