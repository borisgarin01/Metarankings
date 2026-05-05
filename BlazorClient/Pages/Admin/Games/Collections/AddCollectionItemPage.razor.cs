using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Games.Collections;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Collections;

public partial class AddCollectionItemPage : ComponentBase
{
    public IEnumerable<Game> GamesToSelectFrom { get; set; }

    public long SelectedGameId { get; private set; }

    [Parameter, EditorRequired]
    public long Id { get; set; }

    [Inject]
    public IWebManager<Game, AddGameModel, UpdateGameModel> GamesWebManager { get; set; }

    [Inject]
    public IWebManager<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel> GamesCollectionsItemsWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        GamesToSelectFrom = await GamesWebManager.GetAllAsync();
    }

    public Task SelectGame(ChangeEventArgs e)
    {
        SelectedGameId = long.Parse((string)e.Value);

        return Task.CompletedTask;
    }

    public async Task AddGameCollectionItemAsync()
    {
        HttpResponseMessage httpResponseMessage = await GamesCollectionsItemsWebManager.AddAsync(new AddGamesCollectionItemModel(SelectedGameId, Id));
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo($"/admin/games/collections/{Id}/manage-collection");
        else
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
