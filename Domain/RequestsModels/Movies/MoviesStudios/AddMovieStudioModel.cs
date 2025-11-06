namespace Domain.RequestsModels.Movies.MoviesStudios;

public sealed record AddMovieStudioModel
(
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name max length is 255")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    string Name
);