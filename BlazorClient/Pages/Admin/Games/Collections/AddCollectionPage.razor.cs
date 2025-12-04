using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Games.Collections;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Collections;

public partial class AddCollectionPage : ComponentBase
{
    [Inject]
    public IWebManager<Game, AddGameModel, UpdateGameModel> GamesWebManager { get; set; }

    [Inject]
    public IWebManager<GameCollection, AddGameCollectionModel, UpdateGameCollectionModel> GamesCollectionsWebManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string CollectionName { get; set; }

    [Required]
    [MaxLength(4000)]
    [MinLength(1)]
    public string Description { get; set; }

    public IEnumerable<Game> GamesToSelectFrom { get; set; }

    public IEnumerable<long> SelectedGamesIds { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        GamesToSelectFrom = await GamesWebManager.GetAllAsync();
    }

    public Task SelectGames(ChangeEventArgs e)
    {
        SelectedGamesIds = ((string[])e.Value)
            .Select(idString => long.Parse(idString))
            .ToList();
        return Task.CompletedTask;
    }

    public async Task AddGameCollectionAsync()
    {
        var addGameCollectionModel = new AddGameCollectionModel(CollectionName, Description, SelectedGamesIds);

        HttpResponseMessage gameCreationHttpResponseMessage = await GamesCollectionsWebManager.AddAsync(addGameCollectionModel);

        if (!gameCreationHttpResponseMessage.IsSuccessStatusCode)
            await JSRuntime.InvokeVoidAsync("alert", await gameCreationHttpResponseMessage.Content.ReadAsStringAsync());

        else
            NavigationManager.NavigateTo("/games/collections");
    }
}
