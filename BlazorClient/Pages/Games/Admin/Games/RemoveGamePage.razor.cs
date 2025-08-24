using Domain.Games;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Games.Developers;
using WebManagers;

namespace BlazorClient.Pages.Games.Admin.Games
{
    public partial class RemoveGamePage : ComponentBase
    {
        [Parameter]
        public long Id { get; set; }

        public Game Game { get; private set; }

        [Inject]
        public IWebManager<Game, AddGameModel, UpdateGameModel> GamesWebManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Game = await GamesWebManager.GetAsync(Id);
        }
    }
}
