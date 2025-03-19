using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents;

public class GameImage
{
    [Parameter]
    public long Id { get; set; }

    [Parameter]
    public long GameId { get; set; }
    
    [Parameter]
    public required string Href { get; set; }
}
