namespace Domain.RequestsModels.Games.Collections;

public sealed record AddGamesCollectionItemModel
    ([property: JsonPropertyName("gameId")] long GameId,
    [property: JsonPropertyName("gameCollectionId")] long GameCollectionId);
