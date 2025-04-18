﻿using Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GamePlatforms : ComponentBase
{
    [Parameter]
    public IEnumerable<Platform> Platforms { get; set; }
}
