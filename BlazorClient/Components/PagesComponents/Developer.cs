using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents;

public class Developer
{
    [Parameter]
    public long Id { get; set; }

    [Parameter]
    public required string Name { get; set; }
}
