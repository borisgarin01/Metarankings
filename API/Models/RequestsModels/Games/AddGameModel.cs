using API.Models.RequestsModels.Games.Developers;
using API.Models.RequestsModels.Games.Genres;
using API.Models.RequestsModels.Games.Localizations;
using API.Models.RequestsModels.Games.Platforms;
using API.Models.RequestsModels.Games.Publishers;

namespace API.Models.RequestsModels.Games;

public sealed record AddGameModel(
    [property : JsonPropertyName("name")]
    [Required(ErrorMessage = "Name should be set")]
    [MaxLength(511, ErrorMessage ="Name's max length is 511")]
    [MinLength(1, ErrorMessage ="Name should be not empty")]
    string Name,

    [property: JsonPropertyName("image")]
    [Required(ErrorMessage = "Image should be set")]
    [MaxLength(511, ErrorMessage ="Image's max length is 1023")]
    [MinLength(1, ErrorMessage ="Image should be not empty")] string Image,

    [property: JsonPropertyName("developers")]
    [Required(ErrorMessage ="Developers should be set")]
    [MinLength(1,ErrorMessage ="Developers should be not empty")]
    IEnumerable<AddDeveloperModel> Developers,

    [property: JsonPropertyName("publisher")]
    [Required(ErrorMessage ="Publisher should be set")]
    AddPublisherModel Publisher,

    [property: JsonPropertyName("genres")]
    [Required(ErrorMessage ="Genres should be set")]
    [MinLength(1,ErrorMessage ="Genres should be not empty")]
    IEnumerable<AddGenreModel> Genres,

    [property: JsonPropertyName("localization")]
    [Required(ErrorMessage ="Localization should be set")]
    AddLocalizationModel Localization,

    [property: JsonPropertyName("releaseDate")]
    [Required(ErrorMessage ="Release date should be set")]
    [Range(typeof(DateTime), "1/1/1940", "1/1/2200",
        ErrorMessage = "Value for {0} must be between {1} and {2}")]
    DateTime? ReleaseDate,

    [property:JsonPropertyName("description")]
    [MinLength(1, ErrorMessage ="Description should be not empty")]
    string Description,

    [property: JsonPropertyName("trailer")]
    string Trailer,
    [property: JsonPropertyName("platforms")]
    IEnumerable<AddPlatformModel> Platforms);
