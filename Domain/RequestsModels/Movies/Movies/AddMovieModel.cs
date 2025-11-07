namespace Domain.RequestsModels.Movies.Movies;

public sealed record AddMovieModel(string Name, string OriginalName, string Description, string ImageSource, DateTime PremierDate, IEnumerable<long> MoviesDirectorsIds, IEnumerable<long> MoviesGenresIds, IEnumerable<long> MoviesStudiosIds);
