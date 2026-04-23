using System.Text.Json.Serialization;

namespace ViewModels;

public sealed record MovieReviewListViewModel(
    [property: JsonPropertyName("movieId")] long MovieId,
    [property: JsonPropertyName("movieName")] string MovieName);
