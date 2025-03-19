﻿namespace Domain;
public sealed record Game
{
    public long Id { get; set; }
    public required string Href { get; set; }
    public required string Name { get; set; }
    public required IEnumerable<GameImage> Images { get; set; }
    public float? Score { get; set; }
    public long? ScoresCount { get; set; }
    public required IEnumerable<Developer> Developers { get; set; }
    public required Publisher Publisher { get; set; }
    public required IEnumerable<Platform> Platforms { get; set; }
    public required IEnumerable<Genre> Genres { get; set; }
    public required Localization Localization { get; set; }
    public long LocalizationId { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public required string Description { get; set; }
}
