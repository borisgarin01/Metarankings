using Domain.Games;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GamePublishersComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Publisher> Publishers { get; set; }
}
