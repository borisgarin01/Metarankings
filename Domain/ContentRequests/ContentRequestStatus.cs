namespace Domain.ContentRequests;

/// <summary>
/// Состояние заявки на добавление игры/фильма.
/// </summary>
public enum ContentRequestStatus : short
{
    /// <summary>Новая, ещё не рассмотрена администратором.</summary>
    Pending = 0,

    /// <summary>Администратор добавил игру/фильм на сайт.</summary>
    Accepted = 1,

    /// <summary>Администратор отклонил заявку.</summary>
    Rejected = 2
}
