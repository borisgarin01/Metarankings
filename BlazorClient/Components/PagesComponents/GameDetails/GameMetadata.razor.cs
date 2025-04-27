using Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameMetadata : ComponentBase
{
    [Parameter] public IEnumerable<Developer> Developers { get; set; }
    [Parameter] public Publisher Publisher { get; set; }
    [Parameter] public IEnumerable<Platform> Platforms { get; set; }
    [Parameter] public IEnumerable<Genre> Genres { get; set; }
    [Parameter] public Localization Localization { get; set; }
    [Parameter] public DateTime? ReleaseDate { get; set; }
}
