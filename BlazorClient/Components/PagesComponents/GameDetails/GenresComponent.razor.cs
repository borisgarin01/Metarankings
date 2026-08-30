using Domain.Games;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GenresComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Genre> Genres { get; set; } = Enumerable.Empty<Genre>();
}
