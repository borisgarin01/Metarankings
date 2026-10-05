using Domain.ContentRequests;

namespace BlazorClient.Components.PagesComponents.ContentRequests;

/// <summary>
/// Подписи для типов и статусов заявок на добавление.
/// </summary>
public static class ContentRequestLabels
{
    public static string GetTypeLabel(ContentRequestType contentType) => contentType switch
    {
        ContentRequestType.Game => "Игра",
        ContentRequestType.Movie => "Фильм",
        _ => contentType.ToString()
    };

    public static string GetStatusLabel(ContentRequestStatus status) => status switch
    {
        ContentRequestStatus.Pending => "На рассмотрении",
        ContentRequestStatus.Accepted => "Добавлено",
        ContentRequestStatus.Rejected => "Отклонено",
        _ => status.ToString()
    };
}
