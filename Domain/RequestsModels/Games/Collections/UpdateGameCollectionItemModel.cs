namespace Domain.RequestsModels.Games.Collections;

public sealed record UpdateGameCollectionItemModel
    ([property: JsonPropertyName("gameId")] long GameId,
    [property: JsonPropertyName("gameCollectionId")] long GameCollectionId);
