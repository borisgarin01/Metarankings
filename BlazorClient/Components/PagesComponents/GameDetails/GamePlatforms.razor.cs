using Domain.Games;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GamePlatforms : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Platform> Platforms { get; set; } = Enumerable.Empty<Platform>();
}
