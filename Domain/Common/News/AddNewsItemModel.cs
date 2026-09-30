namespace Domain.Common.News;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Модель для создания новости, приходит от клиента.
/// UserId отсутствует — берётся из JWT-claims на сервере.
/// </summary>
public sealed class AddNewsItemModel
{
    [Required, MaxLength(511), MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string TextContent { get; set; } = string.Empty;

    [Required]
    public string ImageSource { get; set; } = string.Empty;
}
