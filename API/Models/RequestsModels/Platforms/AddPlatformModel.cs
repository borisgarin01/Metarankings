﻿using System.Text.Json.Serialization;

namespace API.Models.RequestsModels.Platforms;

public sealed record AddPlatformModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("href")]
    public string Href { get; set; }
}
