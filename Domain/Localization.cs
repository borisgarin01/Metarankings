using Microsoft.AspNetCore.Components;

namespace Domain;
public sealed record Localization
{
    [Parameter]
    public string Link { get; set; }

    [Parameter]
    public string Name { get; set; }
}
