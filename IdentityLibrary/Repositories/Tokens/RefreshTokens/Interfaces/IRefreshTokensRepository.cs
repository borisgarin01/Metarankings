using IdentityLibrary.DTOs;

namespace IdentityLibrary.Repositories.Tokens.RefreshTokens.Interfaces;

public interface IRefreshTokensRepository
{
    /// <summary>
    /// Создает новый refresh token
    /// </summary>
    Task<RefreshToken> CreateAsync(RefreshToken token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает неотозванный токен по значению
    /// </summary>
    Task<RefreshToken?> GetByValueAsync(string value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает все токены пользователя
    /// </summary>
    Task<IEnumerable<RefreshToken>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает последний неотозванный токен пользователя
    /// </summary>
    Task<RefreshToken?> GetLastActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отзывает конкретный токен
    /// </summary>
    Task RevokeAsync(long tokenId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Атомарно отзывает токен, если он ещё не отозван.
    /// Возвращает false, если токен уже был отозван (например, параллельным запросом).
    /// </summary>
    Task<bool> TryRevokeAsync(long tokenId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отзывает все токены пользователя
    /// </summary>
    Task RevokeAllByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет все отозванные токены (для очистки БД)
    /// </summary>
    Task DeleteRevokedAsync(CancellationToken cancellationToken = default);
}
