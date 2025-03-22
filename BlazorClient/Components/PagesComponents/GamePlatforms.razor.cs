using Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents;

public partial class GamePlatforms
{
    [Parameter]
    public Platform[] Platforms { get; set; }
}
