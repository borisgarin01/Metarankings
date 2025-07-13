using Domain.Games;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameDevelopers : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Developer> Developers { get; set; } = Enumerable.Empty<Developer>();
}
