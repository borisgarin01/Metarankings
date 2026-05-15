using Domain.Games;
using IdentityLibrary.DTOs;

namespace Domain.Reviews;

public sealed record GameReview
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("gameId")]
    public long GameId { get; set; }

    [JsonPropertyName("game")]
    public Game Game { get; set; }

    [JsonPropertyName("applicationUser")]
    public ApplicationUser ApplicationUser { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("score")]
    [Range(0.0f, 10.0f)]
    public float Score { get; set; }

    [Required(ErrorMessage = "Text should be set")]
    [MinLength(1, ErrorMessage = "Text should be not empty")]
    public string TextContent { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    public List<GamePlayerReviewShift> GamePlayerReviewShifts { get; set; } = new();
}