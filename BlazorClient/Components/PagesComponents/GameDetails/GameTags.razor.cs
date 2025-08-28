using Domain.Common;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameTags : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Link> Links { get; set; }
}
