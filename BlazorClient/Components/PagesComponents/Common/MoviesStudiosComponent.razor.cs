using Domain.Movies;

namespace BlazorClient.Components.PagesComponents.Common;

public partial class MoviesStudiosComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<MovieStudio> MoviesStudios { get; set; }
}
