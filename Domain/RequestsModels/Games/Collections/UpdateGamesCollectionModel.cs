namespace Domain.RequestsModels.Games.Collections;

public sealed record UpdateGamesCollectionModel(
[property: JsonPropertyName("collectionName")]
string CollectionName,
[property:JsonPropertyName("imageSource")]
string ImageSource);