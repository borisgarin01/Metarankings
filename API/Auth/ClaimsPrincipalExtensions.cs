namespace API.Auth;

/// <summary>
/// Чтение данных текущего пользователя из claims.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Идентификатор пользователя из claim NameIdentifier; null, если claim отсутствует или некорректен.
    /// </summary>
    public static long? GetUserId(this ClaimsPrincipal user)
    {
        string? userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return long.TryParse(userId, out long parsedUserId) ? parsedUserId : null;
    }
}
