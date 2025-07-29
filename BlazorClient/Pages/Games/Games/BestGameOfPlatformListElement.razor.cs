using Domain.Games;

namespace BlazorClient.Pages.Games.Games;

public partial class BestGameOfPlatformListElement : ComponentBase
{
    [Parameter, EditorRequired]
    public Game Game { get; set; }
}
