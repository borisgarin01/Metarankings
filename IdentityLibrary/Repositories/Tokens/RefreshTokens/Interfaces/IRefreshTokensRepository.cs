using IdentityLibrary.DTOs;

namespace IdentityLibrary.Repositories.Tokens.RefreshTokens.Interfaces;

public interface IRefreshTokensRepository
{
    /// <summary>
    /// Создает новый refresh token
    /// </summary>
    Task<RefreshToken> CreateAsync(RefreshToken token);

    /// <summary>
    /// Получает неотозванный токен по значению
    /// </summary>
    Task<RefreshToken?> GetByValueAsync(string value);

    /// <summary>
    /// Получает все токены пользователя
    /// </summary>
    Task<IEnumerable<RefreshToken>> GetByUserIdAsync(long userId);

    /// <summary>
    /// Получает последний неотозванный токен пользователя
    /// </summary>
    Task<RefreshToken?> GetLastActiveByUserIdAsync(long userId);

    /// <summary>
    /// Отзывает конкретный токен
    /// </summary>
    Task RevokeAsync(long tokenId);

    /// <summary>
    /// Атомарно отзывает токен, если он ещё не отозван.
    /// Возвращает false, если токен уже был отозван (например, параллельным запросом).
    /// </summary>
    Task<bool> TryRevokeAsync(long tokenId);

    /// <summary>
    /// Отзывает все токены пользователя
    /// </summary>
    Task RevokeAllByUserIdAsync(long userId);

    /// <summary>
    /// Удаляет все отозванные токены (для очистки БД)
    /// </summary>
    Task DeleteRevokedAsync();
}
