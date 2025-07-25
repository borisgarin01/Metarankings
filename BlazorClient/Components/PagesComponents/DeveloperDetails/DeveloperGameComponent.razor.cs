﻿using Domain.Games;

namespace BlazorClient.Components.PagesComponents.DeveloperDetails;

public partial class DeveloperGameComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public Game Game { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }
}
