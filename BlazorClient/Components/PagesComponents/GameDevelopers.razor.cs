using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents;

public partial class GameDevelopers
{
    [Parameter]
    public string[] Developers { get; set; }
}
