using Domain.Games;

namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class BestGamesOfPlatformsComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Platform> Platforms { get; set; }

    [CascadingParameter(Name = "PlatformId")]
    public int? PlatformId { get; set; }
}