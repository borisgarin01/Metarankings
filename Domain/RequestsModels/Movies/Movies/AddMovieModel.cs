namespace Domain.RequestsModels.Movies.Movies;

public sealed record AddMovieModel(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("originalName")] string OriginalName,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("imageSource")] string ImageSource,
    [property: JsonPropertyName("premierDate")] DateTime PremierDate,
    [property: JsonPropertyName("moviesDirectorsNames")] IEnumerable<string> MoviesDirectorsNames,
    [property: JsonPropertyName("moviesGenresNames")] IEnumerable<string> MoviesGenresNames,
    [property: JsonPropertyName("moviesStudiosNames")] IEnumerable<string> MoviesStudiosNames);
