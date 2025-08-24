namespace Domain.RequestsModels.Games;

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

    [property: JsonPropertyName("developersIds")]
    [Required(ErrorMessage ="Developers ids should be set")]
    [MinLength(1,ErrorMessage ="Developers ids should be not empty")]
    IEnumerable<long> DevelopersIds,

    [property: JsonPropertyName("publisherId")]
    [Required(ErrorMessage ="Publisher id should be set")]
    long PublisherId,

    [property: JsonPropertyName("genresIds")]
    [Required(ErrorMessage ="Genres ids should be set")]
    [MinLength(1,ErrorMessage ="Genres ids should be not empty")]
    IEnumerable<long> GenresIds,

    [property: JsonPropertyName("localizationId")]
    [Required(ErrorMessage ="LocalizationId should be set")]
    long LocalizationId,

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
    IEnumerable<long> PlatformsIds);
