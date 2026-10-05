using Domain.ContentRequests;

namespace Domain.RequestsModels.ContentRequests;

/// <summary>
/// Решение администратора по заявке.
/// </summary>
public sealed class UpdateContentRequestStatusModel
{
    [EnumDataType(typeof(ContentRequestStatus))]
    public ContentRequestStatus Status { get; set; }

    [MaxLength(2000)]
    public string? AdminComment { get; set; }
}
