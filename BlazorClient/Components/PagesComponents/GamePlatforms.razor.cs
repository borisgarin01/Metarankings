using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents;

public partial class GamePlatforms
{
    [Parameter]
    public string[] Platforms { get; set; }
}
