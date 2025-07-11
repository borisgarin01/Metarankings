﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Games;

[Table("Localizations")]
public sealed record Localization
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("games")]
    public IEnumerable<Game> Games { get; set; } = Enumerable.Empty<Game>();
}
