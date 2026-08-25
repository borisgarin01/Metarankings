using Blazored.Toast.Services;
using Domain.Games;
using WebManagers.Derived.Games;

namespace BlazorClient.Pages.Admin.Games.Games;

public partial class RemoveGamePage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public Game Game { get; private set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public GamesWebManager GamesWebManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Game = await GamesWebManager.GetAsync(Id);
    }

    public async Task RemoveGameAsync()
    {
        HttpResponseMessage httpResponseMessage = await GamesWebManager.DeleteAsync(Id);
        if (httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/games/games/list-games");
        else
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
