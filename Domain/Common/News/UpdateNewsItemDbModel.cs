namespace Domain.Common.News;

/// <summary>
/// Внутренняя модель для обновления новости.
/// Содержит UserId, который устанавливается на сервере из JWT-claims.
/// </summary>
public sealed class UpdateNewsItemDbModel
{
    public string Title { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
    public string ImageSource { get; set; } = string.Empty;
    public long UserId { get; set; }
}