namespace Domain.RequestsModels.Games.Collections;

public sealed record AddGameCollectionModel(
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("description")]
    string Description,
    [property:JsonPropertyName("selectedGamesIds")]
    IEnumerable<long> SelectedGamesIds);