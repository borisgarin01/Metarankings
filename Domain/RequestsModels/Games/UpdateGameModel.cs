using Domain.Common;

namespace Domain.RequestsModels.Games;

public sealed record UpdateGameModel(
    [property: JsonPropertyName("name")]
    [Required(ErrorMessage = "Name should be set")]
    [MaxLength(511, ErrorMessage = "Name's max length is 511")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    string Name,

    [property: JsonPropertyName("image")]
    [Required(ErrorMessage = "Image should be set")]
    [MaxLength(511, ErrorMessage = "Image's max length is 511")]
    [MinLength(1, ErrorMessage = "Image should be not empty")]
    string Image,

    [property: JsonPropertyName("developers")]
    [Required(ErrorMessage = "Developers should be set")]
    [MinLength(1, ErrorMessage = "Developers should be not empty")]
    IEnumerable<OrderedId> Developers,

    [property: JsonPropertyName("publishers")]
    [Required(ErrorMessage = "Publishers should be set")]
    [MinLength(1, ErrorMessage = "Publishers should be not empty")]
    IEnumerable<OrderedId> Publishers,

    [property: JsonPropertyName("genres")]
    [Required(ErrorMessage = "Genres should be set")]
    [MinLength(1, ErrorMessage = "Genres should be not empty")]
    IEnumerable<OrderedId> Genres,

    [property: JsonPropertyName("localizationId")]
    [Required(ErrorMessage = "LocalizationId should be set")]
    long LocalizationId,

    [property: JsonPropertyName("releaseDate")]
    [Required(ErrorMessage = "Release date should be set")]
    [Range(typeof(DateTime), "1/1/1940", "1/1/2200",
        ErrorMessage = "Value for {0} must be between {1} and {2}")]
    DateTime? ReleaseDate,

    [property: JsonPropertyName("description")]
    [MinLength(1, ErrorMessage = "Description should be not empty")]
    string Description,

    [property: JsonPropertyName("trailer")]
    string Trailer,

    [property: JsonPropertyName("platforms")]
    [Required(ErrorMessage = "Platforms should be set")]
    [MinLength(1, ErrorMessage = "Platforms should be not empty")]
    IEnumerable<OrderedId> Platforms);