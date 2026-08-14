using Domain.Movies;

namespace BlazorClient.Components.PagesComponents.Home;

public partial class MoviesDropdownComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<MovieGenre> Genres { get; set; }
}
