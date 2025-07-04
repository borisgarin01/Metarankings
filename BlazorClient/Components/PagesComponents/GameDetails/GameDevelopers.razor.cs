﻿using Domain.Games;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameDevelopers : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Developer> Developers { get; set; } = Enumerable.Empty<Developer>();
}
