using Domain.Games.Collections;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameCollectionsComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<GameCollection> GameCollections { get; set; }
}
