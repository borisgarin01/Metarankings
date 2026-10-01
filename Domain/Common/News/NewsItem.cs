using IdentityLibrary.DTOs;

namespace Domain.Common.News;

public sealed record NewsItem
{
    public long Id { get; set; }

    [Required, MaxLength(511), MinLength(1)]
    public string Title { get; set; }

    public long UserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    [Required]
    public string TextContent { get; set; }

    [Required]
    public string ImageSource { get; set; }

    public DateTime PublishTimestamp { get; set; }
}

