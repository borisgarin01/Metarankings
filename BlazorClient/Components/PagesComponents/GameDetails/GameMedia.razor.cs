using Domain.Games;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameMedia : ComponentBase
{
    [Parameter, EditorRequired]
    public long GameId { get; set; }

    [Parameter, EditorRequired]
    public string Trailer { get; set; }

    [Parameter, EditorRequired]
    public IEnumerable<GameScreenshot> Screenshots { get; set; } = Enumerable.Empty<GameScreenshot>();

    [Parameter, EditorRequired]
    public string Name { get; set; }

    [Parameter, EditorRequired]
    public string Image { get; set; }

    [Parameter, EditorRequired]
    public DateTime? ReleaseDate { get; set; }
}
