using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameHeader : ComponentBase
{
    [Parameter] public string Name { get; set; }
    [Parameter] public string Description { get; set; }
    [Parameter] public int Id { get; set; }
    [Parameter] public int? ReleaseYear { get; set; }
}
