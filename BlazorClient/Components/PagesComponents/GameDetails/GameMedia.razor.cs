using Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameMedia : ComponentBase
{
    [Parameter] public string Trailer { get; set; }
    [Parameter] public IEnumerable<Screenshot> Screenshots { get; set; }
    [Parameter] public string Name { get; set; }
    [Parameter] public string Image { get; set; }
    [Parameter] public string Href { get; set; }
    [Parameter] public DateOnly? ReleaseDate { get; set; }
}
