using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents;

public partial class GameModel
{
    [Parameter]
    public Domain.Game Game { get; set; }
}
