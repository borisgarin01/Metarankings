namespace Domain.RequestsModels.Games.Collections;

public sealed record AddGameCollectionItemModel
    ([property: JsonPropertyName("gameId")] long GameId,
    [property: JsonPropertyName("gameCollectionId")] long GameCollectionId);
