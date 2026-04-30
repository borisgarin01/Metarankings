namespace Domain.RequestsModels.Games.Collections;

public sealed record UpdateGamesCollectionItemModel
    ([property: JsonPropertyName("gameId")] long GameId,
    [property: JsonPropertyName("gameCollectionId")] long GameCollectionId);
