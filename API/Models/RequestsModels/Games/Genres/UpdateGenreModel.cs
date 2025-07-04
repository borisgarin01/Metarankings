﻿using System.Text.Json.Serialization;

namespace API.Models.RequestsModels.Games.Genres;

public sealed record UpdateGenreModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
