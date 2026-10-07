using Domain.RequestsModels.Games;
using WebManagers;
using Domain.Games;

namespace BlazorClient.Pages.Admin.Games.Games;

public partial class ListGamesPage : ComponentBase
{
    public IEnumerable<Game> Games { get; private set; }

    [Inject]
    public IWebManager<Game, AddGameModel, UpdateGameModel> WebManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Games = await WebManager.GetAllAsync();
    }
}
