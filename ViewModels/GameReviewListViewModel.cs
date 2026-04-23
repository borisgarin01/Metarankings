using System.Text.Json.Serialization;

namespace ViewModels;

public sealed record GameReviewListViewModel(
    [property: JsonPropertyName("gameId")] long GameId,
    [property: JsonPropertyName("gameName")] string GameName);