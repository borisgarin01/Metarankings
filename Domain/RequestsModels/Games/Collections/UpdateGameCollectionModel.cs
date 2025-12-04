namespace Domain.RequestsModels.Games.Collections;

public sealed record UpdateGameCollectionModel(
[property: JsonPropertyName("collectionName")]
string CollectionName);