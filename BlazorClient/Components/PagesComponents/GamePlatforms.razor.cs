﻿using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents;

public partial class GamePlatforms
{
    [Parameter]
    public IEnumerable<string> Platforms { get; set; }
}
