namespace API.Models.RequestsModels.Games.Genres;

public sealed record UpdateGenreModel
    ([Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Max length is 255")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    [property:JsonPropertyName("name")]
    string Name);
