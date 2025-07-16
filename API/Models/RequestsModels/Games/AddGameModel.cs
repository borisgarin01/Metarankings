using API.Models.RequestsModels.Games.Developers;
using API.Models.RequestsModels.Games.Genres;
using API.Models.RequestsModels.Games.Localizations;
using API.Models.RequestsModels.Games.Platforms;
using API.Models.RequestsModels.Games.Publishers;

namespace API.Models.RequestsModels.Games;

public sealed record AddGameModel(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("image")] string Image,
    [property: JsonPropertyName("developers")] IEnumerable<AddDeveloperModel> Developers,
    [property: JsonPropertyName("publisher")] AddPublisherModel Publisher,
    [property: JsonPropertyName("genres")] IEnumerable<AddGenreModel> Genres,
    [property: JsonPropertyName("localization")] AddLocalizationModel Localization,
    [property: JsonPropertyName("releaseDate")] DateTime? ReleaseDate,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("trailer")] string Trailer,
    [property: JsonPropertyName("platforms")] IEnumerable<AddPlatformModel> Platforms);
