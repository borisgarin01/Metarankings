using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameThumbnail : ComponentBase
{
    [Parameter, EditorRequired]
    public string Image { get; set; }

    [Parameter, EditorRequired]
    public string Name { get; set; }

    [Parameter, EditorRequired]
    public int? ReleaseYear { get; set; }
}
