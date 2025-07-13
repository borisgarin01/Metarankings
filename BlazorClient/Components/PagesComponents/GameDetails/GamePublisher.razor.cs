using Domain.Games;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GamePublisher : ComponentBase
{
    [Parameter, EditorRequired]
    public Publisher Publisher { get; set; }
}
