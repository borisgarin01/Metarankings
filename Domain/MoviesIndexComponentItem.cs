﻿namespace Domain;

public sealed record MoviesIndexComponentItem
{
    public IndexComponentItem IndexComponentItem { get; set; }
    public Genre[] Genres { get; set; }
}