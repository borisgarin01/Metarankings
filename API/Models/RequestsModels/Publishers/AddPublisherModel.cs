﻿using System.Text.Json.Serialization;

namespace API.Models.RequestsModels.Publishers;

public sealed record AddPublisherModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
