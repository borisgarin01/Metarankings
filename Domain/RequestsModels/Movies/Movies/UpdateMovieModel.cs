namespace Domain.RequestsModels.Movies.Movies;

public sealed record UpdateMovieModel(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("originalName")] string OriginalName,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("imageSource")] string ImageSource,
    [property: JsonPropertyName("premierDate")] DateTime PremierDate,
    [property: JsonPropertyName("moviesDirectorsIds")] IEnumerable<long> MoviesDirectorsIds,
    [property: JsonPropertyName("moviesGenresIds")] IEnumerable<long> MoviesGenresIds,
    [property: JsonPropertyName("moviesStudiosIds")] IEnumerable<long> MoviesStudiosIds,
    [property: JsonPropertyName("trailer")] string? Trailer = null);
