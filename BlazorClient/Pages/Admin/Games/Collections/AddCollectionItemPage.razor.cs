using Blazored.Toast.Services;
using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using Microsoft.AspNetCore.Components;
using WebManagers;
using WebManagers.Derived.Games;

namespace BlazorClient.Pages.Admin.Games.Collections;

public partial class AddCollectionItemPage : ComponentBase
{
    private string searchTerm = string.Empty;
    private List<Game> filteredGames = new();

    public IEnumerable<Game> GamesToSelectFrom { get; set; } = Enumerable.Empty<Game>();

    public long SelectedGameId { get; set; }

    [Parameter, EditorRequired]
    public long Id { get; set; }

    [Inject]
    public GamesWebManager GamesWebManager { get; set; } = default!;

    [Inject]
    public IWebManager<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel> GamesCollectionsItemsWebManager { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    private string SearchTerm
    {
        get => searchTerm;
        set
        {
            if (searchTerm == value)
                return;

            searchTerm = value;
            FilterGames();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        GamesToSelectFrom = await GamesWebManager.GetAllAsync();
        filteredGames = GamesToSelectFrom.ToList();
    }

    public async Task AddGameCollectionItemAsync()
    {
        HttpResponseMessage httpResponseMessage =
            await GamesCollectionsItemsWebManager.AddAsync(new AddGamesCollectionItemModel(SelectedGameId, Id));

        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo($"/admin/games/collections/{Id}/manage-collection");
        else
            ToastService.ShowWarning(await httpResponseMessage.Content.ReadAsStringAsync());
    }

    private void FilterGames()
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
        {
            filteredGames = GamesToSelectFrom.ToList();
        }
        else
        {
            filteredGames = GamesToSelectFrom
                .Where(g => g.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!filteredGames.Any(g => g.Id == SelectedGameId))
            SelectedGameId = -1;
    }
}