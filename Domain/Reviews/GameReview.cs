using IdentityLibrary.DTOs;

namespace Domain.Reviews;

public sealed record GameReview
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("gameId")]
    public long GameId { get; init; }

    [JsonPropertyName("applicationUser")]
    public ApplicationUser ApplicationUser { get; init; }

    [JsonPropertyName("userId")]
    public long UserId { get; init; }

    [JsonPropertyName("score")]
    [Range(0.0f, 10.0f)]
    public float Score { get; init; }

    [Required(ErrorMessage = "Text should be set")]
    [MinLength(1, ErrorMessage = "Text should be not empty")]
    public string TextContent { get; init; }

    [JsonPropertyName("date")]
    public DateTime Date { get; init; }
}