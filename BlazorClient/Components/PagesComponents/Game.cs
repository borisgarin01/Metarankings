using Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents;

public sealed class Game
{
    [Parameter]
    public long Id { get; set; }
    
    [Parameter]
    public required string Href { get; set; }
    
    [Parameter]
    public required string Name { get; set; }

    [Parameter]
    public required IEnumerable<GameImage> Images { get; set; }
    
    [Parameter]
    public float? Score { get; set; }
    
    [Parameter]
    public long? ScoresCount { get; set; }
    
    [Parameter]
    public required IEnumerable<Developer> Developers { get; set; }
    
    [Parameter]
    public required Publisher Publisher { get; set; }
    
    [Parameter]
    public required IEnumerable<Platform> Platforms { get; set; }
    
    [Parameter]
    public required IEnumerable<Genre> Genres { get; set; }
    
    [Parameter]
    public required Localization Localization { get; set; }
    
    [Parameter]
    public long LocalizationId { get; set; }
    
    [Parameter]
    public DateTime? ReleaseDate { get; set; }
    
    [Parameter]
    public required string Description { get; set; }
}
