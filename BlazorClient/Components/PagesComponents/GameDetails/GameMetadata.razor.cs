using Domain;
using Microsoft.AspNetCore.Components;
using System;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameMetadata : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Developer> Developers { get; set; } = Enumerable.Empty<Developer>();

    [Parameter, EditorRequired]
    public Publisher Publisher { get; set; }

    [Parameter, EditorRequired]
    public IEnumerable<Platform> Platforms { get; set; } = Enumerable.Empty<Platform>();

    [Parameter, EditorRequired]
    public IEnumerable<Genre> Genres { get; set; } = Enumerable.Empty<Genre>();

    [Parameter, EditorRequired]
    public Localization Localization { get; set; }

    [Parameter, EditorRequired]
    public DateTime? ReleaseDate { get; set; }
}
