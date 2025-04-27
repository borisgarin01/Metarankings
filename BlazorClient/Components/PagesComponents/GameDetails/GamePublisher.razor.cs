using Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GamePublisher
{
    [Parameter]
    public Publisher Publisher { get; set; }
}
