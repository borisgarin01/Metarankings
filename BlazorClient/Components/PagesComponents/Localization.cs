﻿using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents;

public class Localization
{
    [Parameter]
    public long Id { get; set; }

    [Parameter]
    public string Name { get; set; }
}
