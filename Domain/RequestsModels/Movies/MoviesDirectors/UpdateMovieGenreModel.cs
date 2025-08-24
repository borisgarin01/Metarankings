namespace Domain.RequestsModels.Movies.MoviesDirectors;

public sealed record UpdateMovieGenreModel
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name max length is 255")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    public string Name { get; set; }
}
