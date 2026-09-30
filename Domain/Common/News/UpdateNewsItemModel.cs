namespace Domain.Common.News;

/// <summary>
/// Модель для обновления новости, приходит от клиента.
/// UserId отсутствует — берётся из JWT-claims на сервере.
/// </summary>
public sealed class UpdateNewsItemModel
{
    [Required, MaxLength(511), MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string TextContent { get; set; } = string.Empty;

    [Required]
    public string ImageSource { get; set; } = string.Empty;
}
