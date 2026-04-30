namespace Domain.RequestsModels.Movies.Collections;

public sealed record UpdateMoviesCollectionItemModel
    ([property: JsonPropertyName("movieId")] long MovieId,
    [property: JsonPropertyName("moviesCollectionId")] long MoviesCollectionId);
