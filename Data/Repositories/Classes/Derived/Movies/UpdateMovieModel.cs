using Domain.RequestsModels.Movies.MoviesDirectors;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed record UpdateMovieModel
{
    public required string Name { get; init; }
    public required string OriginalName { get; init; }
    public required string ImageSource { get; init; }
    public required DateTime PremierDate { get; init; }
    public required string Description { get; init; }

    public List<AddMovieStudioModel> AddMovieStudiosModels { get; } = new();
    public List<AddMovieGenreModel> AddMovieGenresModels { get; } = new();
    public List<AddMovieDirectorModel> AddMovieDirectorsModels { get; } = new();
}