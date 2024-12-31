using Microsoft.AspNetCore.Components;

namespace Domain;
public sealed record Platform
{
    public long Id { get; set; }
    [Parameter]
    public string Name { get; set; }

    [Parameter]
    public string Link { get; set; }
}
