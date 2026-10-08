namespace Domain.RequestsModels.Movies.MoviesPersons;

public sealed record AddMoviePersonModel
    ([Required(ErrorMessage = "Name is required")]
    [MaxLength(511, ErrorMessage = "Max length is 511")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    [property:JsonPropertyName("name")]
    string Name);
