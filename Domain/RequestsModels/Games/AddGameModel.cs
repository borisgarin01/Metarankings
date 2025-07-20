using Domain.RequestsModels.Games.Developers;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Localizations;
using Domain.RequestsModels.Games.Platforms;
using Domain.RequestsModels.Games.Publishers;

namespace Domain.RequestsModels.Games;

public sealed record AddGameModel
{
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
