using Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class Game : ComponentBase
{
    [Parameter, EditorRequired]
    public long Id { get; set; }

    [Parameter, EditorRequired]
    public required string Href { get; set; }

    [Parameter, EditorRequired]
    public required string Name { get; set; }

    [Parameter, EditorRequired]
    public required string Image { get; set; }

    [Parameter, EditorRequired]
    public float? Score { get; set; }

    [Parameter, EditorRequired]
    public long? ScoresCount { get; set; }

    [Parameter, EditorRequired]
    public required string[] Developers { get; set; }

    [Parameter, EditorRequired]
    public required string Publisher { get; set; }

    [Parameter, EditorRequired]
    public required IEnumerable<Platform> Platforms { get; set; }

    [Parameter, EditorRequired]
    public required Genre[] Genres { get; set; }

    [Parameter, EditorRequired]
    public required string Localization { get; set; }

    [Parameter, EditorRequired]
    public DateOnly? ReleaseDate { get; set; }

    [Parameter, EditorRequired]
    public required string Description { get; set; }
}
