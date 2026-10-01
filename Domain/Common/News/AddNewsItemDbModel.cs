namespace Domain.Common.News;

public sealed class AddNewsItemDbModel
{
    public string Title { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
    public string ImageSource { get; set; } = string.Empty;
    public long UserId { get; set; }
    public DateTime PublishTimestamp { get; set; }
}
