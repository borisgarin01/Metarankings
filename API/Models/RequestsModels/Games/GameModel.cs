using API.Models.RequestsModels.Games.Developers;
using API.Models.RequestsModels.Games.Genres;
using API.Models.RequestsModels.Games.Localizations;
using API.Models.RequestsModels.Games.Platforms;
using API.Models.RequestsModels.Games.Publishers;
using Domain.Games;
using System.Text.Json.Serialization;

namespace API.Models.RequestsModels.Games;

public sealed record GameModel
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("image")]
    public required string Image { get; set; }

    [JsonPropertyName("developers")]
    public List<AddDeveloperModel> Developers { get; set; } = new();

    [JsonPropertyName("publisher")]
    public required AddPublisherModel Publisher { get; set; }

    [JsonPropertyName("genres")]
    public List<AddGenreModel> Genres { get; set; } = new();

    [JsonPropertyName("localization")]
    public required AddLocalizationModel Localization { get; set; }

    [JsonPropertyName("releaseDate")]
    public DateTime? ReleaseDate { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("trailer")]
    public string Trailer { get; set; }

    [JsonPropertyName("platforms")]
    public List<AddPlatformModel> Platforms { get; set; } = new();
}
