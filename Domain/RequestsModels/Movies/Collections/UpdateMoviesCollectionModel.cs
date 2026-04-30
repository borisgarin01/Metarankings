namespace Domain.RequestsModels.Movies.Collections;

public sealed record UpdateMoviesCollectionModel(
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("description")]
    string Description,
    [property:JsonPropertyName("imageSource")]
    string ImageSource,
    [property:JsonPropertyName("selectedMoviesIds")]
    IEnumerable<long> SelectedMoviesIds);