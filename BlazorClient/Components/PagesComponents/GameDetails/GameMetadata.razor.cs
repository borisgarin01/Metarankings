using Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameMetadata : ComponentBase
{
    [Parameter] public IEnumerable<string> Developers { get; set; }
    [Parameter] public string Publisher { get; set; }
    [Parameter] public IEnumerable<Platform> Platforms { get; set; }
    [Parameter] public IEnumerable<Genre> Genres { get; set; }
    [Parameter] public string Localization { get; set; }
    [Parameter] public DateOnly? ReleaseDate { get; set; }
}
