namespace Domain.RequestsModels.Games.Collections;

public sealed record AddGamesCollectionModel(
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("description")]
    string Description,
    [property:JsonPropertyName("imageSource")]
    string ImageSource,
    [property:JsonPropertyName("selectedGamesIds")]
    IEnumerable<long> SelectedGamesIds);