namespace Domain.Common.News;

public sealed class UpdateNewsItemModel
{
    [Required, MaxLength(511), MinLength(1)]
    public string Title { get; set; }

    public long UserId { get; set; }

    [Required]
    public string TextContent { get; set; }

    [Required]
    public string ImageSource { get; set; }
}
