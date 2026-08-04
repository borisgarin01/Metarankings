using Domain.Games;

namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class GameListItemComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public Game Game { get; set; }
}
