using IdentityLibrary.DTOs;

namespace Domain.Reviews;

public sealed record GameReview
{
    public long Id { get; set; }

    public long GameId { get; set; }

    [Range(0.0f, 10.0f)]
    public float Score { get; set; }

    public ApplicationUser ApplicationUser { get; set; }
    public long UserId { get; set; }

    [Required(ErrorMessage = "Text should be set")]
    [MinLength(1, ErrorMessage = "Text should be not empty")]
    public string TextContent { get; set; }

    public DateTime Date { get; set; }
}