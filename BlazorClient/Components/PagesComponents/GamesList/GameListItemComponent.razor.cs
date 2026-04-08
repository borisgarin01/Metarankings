using Domain.Games;

namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class GameListItemComponent : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }

    [Parameter, EditorRequired]
    public Game Game { get; set; }
}
