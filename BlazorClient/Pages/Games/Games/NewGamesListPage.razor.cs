using Domain.Games;
using Domain.RequestsModels.Games;
using WebManagers;

namespace BlazorClient.Pages.Games.Games;

public partial class NewGamesListPage : ComponentBase
{
    [Inject]
    public IWebManager<Game, AddGameModel, UpdateGameModel> GamesWebManager { get; set; }

    public IEnumerable<Game> Games { get; set; } = Enumerable.Empty<Game>();

    [Parameter]
    public int? PageSize { get; set; } = 5; // Default value

    [Parameter]
    public int? PageNumber { get; set; } = 1; // Default value

    protected override async Task OnParametersSetAsync()
    {
        if (PageNumber < 1 || !PageNumber.HasValue)
            PageNumber = 1;
        if (PageSize < 1 || !PageSize.HasValue)
            PageSize = 5;
        // Fetch data based on the current PageSize and PageNumber
        Games = await GamesWebManager.GetLastAsync((PageNumber.Value - 1) * PageSize.Value, PageSize.Value);
    }
}
