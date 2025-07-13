using Domain.Games;

namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class PlatformsComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public required IEnumerable<Platform> Platforms { get; set; }
}
