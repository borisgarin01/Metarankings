namespace API.Caching;

/// <summary>
/// Имена политик output-кэша.
/// </summary>
public static class CachePolicies
{
    /// <summary>
    /// Публичные GET-эндпоинты, ответ которых не зависит от пользователя. См. <see cref="PublicReadCachePolicy"/>.
    /// </summary>
    public const string PublicRead = nameof(PublicRead);
}
