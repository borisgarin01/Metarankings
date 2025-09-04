using Domain.Games;
using Domain.RequestsModels.Games;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Games
{
    public partial class RemoveGamePage : ComponentBase
    {
        [Parameter]
        public long Id { get; set; }

        public Game Game { get; private set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public IWebManager<Game, AddGameModel, UpdateGameModel> GamesWebManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Game = await GamesWebManager.GetAsync(Id);
        }
        public async Task RemoveGameAsync()
        {
            HttpResponseMessage httpResponseMessage = await GamesWebManager.DeleteAsync(Id);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                NavigationManager.NavigateTo("/admin/games/games/list-games");
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
            }
        }
    }
}
