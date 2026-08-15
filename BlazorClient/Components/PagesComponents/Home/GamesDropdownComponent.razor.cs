using Domain.Games;

namespace BlazorClient.Components.PagesComponents.Home;

public partial class GamesDropdownComponent : ComponentBase
{
    private bool isGamesOpen;
    private string gamesDropdownClass => isGamesOpen ? "show" : "";

    private void ShowGames() => isGamesOpen = true;
    private void HideGames() => isGamesOpen = false;

    [Parameter, EditorRequired]
    public IEnumerable<Platform> Platforms { get; set; }

    [Parameter, EditorRequired]
    public IEnumerable<Genre> Genres { get; set; }
}
