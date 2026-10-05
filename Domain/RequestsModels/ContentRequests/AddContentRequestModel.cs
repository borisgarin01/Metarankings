using Domain.ContentRequests;

namespace Domain.RequestsModels.ContentRequests;

/// <summary>
/// Заявка на добавление от клиента. UserId берётся на сервере из JWT-claims.
/// </summary>
public sealed class AddContentRequestModel
{
    [Required(ErrorMessage = "Выберите, что нужно добавить")]
    [EnumDataType(typeof(ContentRequestType), ErrorMessage = "Выберите игру или фильм")]
    public ContentRequestType? ContentType { get; set; }

    [Required(ErrorMessage = "Укажите название")]
    [MaxLength(511, ErrorMessage = "Название не длиннее 511 символов")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000, ErrorMessage = "Описание не длиннее 4000 символов")]
    public string? Description { get; set; }
}
