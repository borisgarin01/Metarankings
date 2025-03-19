using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents;

public partial class GamesGenres
{
    [Parameter]
    public IEnumerable<string> Genres { get; set; }
}
