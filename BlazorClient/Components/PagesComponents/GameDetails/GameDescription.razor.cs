using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameDescription : ComponentBase
{
    [Parameter] public string Description { get; set; }
}
