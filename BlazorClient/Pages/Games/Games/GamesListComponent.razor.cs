using Domain.Games;

namespace BlazorClient.Pages.Games.Games;

public partial class GamesListComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Game> Games { get; set; }
}
