using Domain.Games;

namespace BlazorClient.Components.PagesComponents.Home;

public partial class GamesDropdownComponent : ComponentBase
{

    [Parameter, EditorRequired]
    public IEnumerable<Platform> Platforms { get; set; }

    [Parameter, EditorRequired]
    public IEnumerable<Genre> Genres { get; set; }
}
