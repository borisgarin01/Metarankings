using Microsoft.AspNetCore.Components;

namespace Domain;

public sealed record FeaturedGame
{
    [Parameter]
    public Developer[] Developers { get; set; }

    [Parameter]
    public Localization Localization { get; set; }

    [Parameter]
    public Publisher[] Publishers { get; set; }

    [Parameter]
    public Platform[] Platforms { get; set; }

    [Parameter]
    public Genre[] Genres { get; set; }

    [Parameter]
    public DateTime ReleaseDate { get; set; }

    [Parameter]
    public string Description { get; set; }
}
