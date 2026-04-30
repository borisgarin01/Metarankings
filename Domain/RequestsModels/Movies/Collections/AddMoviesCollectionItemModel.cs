namespace Domain.RequestsModels.Movies.Collections;

public sealed record AddMoviesCollectionItemModel
    ([property: JsonPropertyName("movieId")] long MovieId,
    [property: JsonPropertyName("moviesCollectionId")] long MoviesCollectionId);
