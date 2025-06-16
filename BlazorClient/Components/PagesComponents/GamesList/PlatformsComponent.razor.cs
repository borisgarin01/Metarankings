using Domain.Games;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class PlatformsComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public required IEnumerable<Platform> Platforms { get; set; }
}
