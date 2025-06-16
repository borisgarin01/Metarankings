using Domain.Games;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class GenresComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Genre> Genres { get; set; } = Enumerable.Empty<Genre>();
}
