using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents;

public partial class GameDevelopers
{
    [Parameter]
    public IEnumerable<string> Developers { get; set; }
}
