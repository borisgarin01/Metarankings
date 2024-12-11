using Microsoft.AspNetCore.Components;

namespace Domain;
public sealed record Genre
{
    [Parameter]
    public string Name { get; set; }

    [Parameter]
    public string Link { get; set; }
}
