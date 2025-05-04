using Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameMetadata : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Developer> Developers { get; set; }
    
    [Parameter, EditorRequired] 
    public Publisher Publisher { get; set; }
    
    [Parameter, EditorRequired]
    public IEnumerable<Platform> Platforms { get; set; }
    
    [Parameter, EditorRequired]
    public IEnumerable<Genre> Genres { get; set; }
    
    [Parameter, EditorRequired]
    public Localization Localization { get; set; }
    
    [Parameter, EditorRequired]
    public DateTime? ReleaseDate { get; set; }
}
