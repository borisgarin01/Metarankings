﻿namespace Domain;
public sealed record GamesReleasesDatesComponentItem
{
    public string Href { get; set; }
    public string Title { get; set; }
    public string ImageSrc { get; set; }
    public string ImageAlt { get; set; }
    public Platform[] Platforms { get; set; }
    public Genre[] Genres { get; set; }
    public DateTime? ReleaseDate { get; set; }
}