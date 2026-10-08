using IdentityLibrary.DTOs;
using IdentityLibrary.Repositories.Tokens.RefreshTokens.Interfaces;

namespace IdentityLibrary.Repositories.Tokens.RefreshTokens.Classes;

public class RefreshTokensRepository : IRefreshTokensRepository
{
    private readonly string _connectionString;

    public RefreshTokensRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private NpgsqlConnection CreateConnection() => new(_connectionString);

    public async Task<RefreshToken> CreateAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        // Полностью соответствует вашему CREATE TABLE
        const string sql = @"
            INSERT INTO RefreshTokens (UserId, Value, IsRevoked, CreatedAt)
            VALUES (@UserId, @Value, @IsRevoked, @CreatedAt)
            RETURNING Id, UserId, Value, IsRevoked, CreatedAt;";

        using var connection = CreateConnection();
        return await connection.QuerySingleAsync<RefreshToken>(new CommandDefinition(sql, token, cancellationToken: cancellationToken));
    }

    public async Task<RefreshToken?> GetByValueAsync(string value, CancellationToken cancellationToken = default)
    {
        // Ищем по значению, не проверяем срок годности (у вас его нет)
        const string sql = @"
            SELECT Id, UserId, Value, IsRevoked, CreatedAt
            FROM RefreshTokens
            WHERE Value = @Value 
              AND IsRevoked = false
            LIMIT 1;";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<RefreshToken>(new CommandDefinition(sql, new { Value = value }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, UserId, Value, IsRevoked, CreatedAt
            FROM RefreshTokens
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC;";

        using var connection = CreateConnection();
        return await connection.QueryAsync<RefreshToken>(new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
    }

    public async Task<RefreshToken?> GetLastActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        // Получаем последний активный (неотозванный) токен пользователя
        const string sql = @"
            SELECT Id, UserId, Value, IsRevoked, CreatedAt
            FROM RefreshTokens
            WHERE UserId = @UserId 
              AND IsRevoked = false
            ORDER BY CreatedAt DESC
            LIMIT 1;";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<RefreshToken>(new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
    }

    public async Task RevokeAsync(long tokenId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE RefreshTokens
            SET IsRevoked = true
            WHERE Id = @TokenId;";

        using var connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, new { TokenId = tokenId }, cancellationToken: cancellationToken));
    }

    public async Task<bool> TryRevokeAsync(long tokenId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE RefreshTokens
            SET IsRevoked = true
            WHERE Id = @TokenId
              AND IsRevoked = false;";

        using var connection = CreateConnection();
        return await connection.ExecuteAsync(new CommandDefinition(sql, new { TokenId = tokenId }, cancellationToken: cancellationToken)) > 0;
    }

    public async Task RevokeAllByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE RefreshTokens
            SET IsRevoked = true
            WHERE UserId = @UserId 
              AND IsRevoked = false;";

        using var connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
    }

    public async Task DeleteRevokedAsync(CancellationToken cancellationToken = default)
    {
        // Чистим отозванные токены
        const string sql = @"
            DELETE FROM RefreshTokens
            WHERE IsRevoked = true;";

        using var connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }
}
