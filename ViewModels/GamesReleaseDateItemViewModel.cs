using Domain.Common;
using System.Text.Json.Serialization;

namespace ViewModels;

public sealed record GamesReleaseDateItemViewModel
    ([property: JsonPropertyName("href")] string Href,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("imageSource")] string ImageSource,
    [property: JsonPropertyName("imageAlt")] string ImageAlt,
    [property: JsonPropertyName("itemName")] string ItemName,
    [property: JsonPropertyName("platforms")] Link[] Platforms,
    [property: JsonPropertyName("genres")] Link[] Genres,
    [property: JsonPropertyName("releaseDate")] DateTime ReleaseDate);
