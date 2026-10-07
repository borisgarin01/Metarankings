using Domain.Waitings;
using System.Text.RegularExpressions;

namespace Data.Repositories.Classes.Derived.Waitings;

/// <summary>
/// Голоса "жду / не жду" для игр и фильмов.
/// Имена таблиц и колонок подставляются в текст SQL (параметризовать идентификаторы в Postgres нельзя), поэтому:
/// наследовать класс можно только внутри сборки Data (private protected), а каждый идентификатор проверяется в конструкторе.
/// Все значения из запросов передаются только параметрами.
/// </summary>
public abstract class WaitingsRepository : Repository
{
    private static readonly Regex SqlIdentifierRegex = new Regex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

    private readonly string _waitingsTable;
    private readonly string _entityColumn;
    private readonly string _entitiesTable;
    private readonly string _releaseDateColumn;

    private protected WaitingsRepository(string connectionString, string waitingsTable, string entityColumn, string entitiesTable, string releaseDateColumn)
        : base(connectionString)
    {
        _waitingsTable = EnsureSqlIdentifier(waitingsTable, nameof(waitingsTable));
        _entityColumn = EnsureSqlIdentifier(entityColumn, nameof(entityColumn));
        _entitiesTable = EnsureSqlIdentifier(entitiesTable, nameof(entitiesTable));
        _releaseDateColumn = EnsureSqlIdentifier(releaseDateColumn, nameof(releaseDateColumn));
    }

    private static string EnsureSqlIdentifier(string identifier, string parameterName)
    {
        if (identifier is null || !SqlIdentifierRegex.IsMatch(identifier))
            throw new ArgumentException($"Недопустимый SQL-идентификатор: '{identifier}'", parameterName);

        return identifier;
    }

    /// <summary>
    /// Вышла ли игра/фильм (дата выхода наступила). null - записи с таким Id нет.
    /// </summary>
    public async Task<bool?> IsReleasedAsync(long entityId)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<bool?>(
            $"SELECT ({_releaseDateColumn} IS NOT NULL AND {_releaseDateColumn} <= CURRENT_DATE) FROM {_entitiesTable} WHERE Id = @EntityId;",
            new { EntityId = entityId });
    }

    /// <summary>
    /// Голосует за ожидание. Повторный голос с тем же значением отменяет голос,
    /// голос с другим значением заменяет предыдущий.
    /// </summary>
    public async Task<WaitingStatistics> VoteAsync(long entityId, long userId, bool isWaiting)
    {
        using NpgsqlConnection connection = CreateConnection();
        await connection.OpenAsync();
        using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();

        bool? existingVote = await connection.QueryFirstOrDefaultAsync<bool?>(
            $"SELECT IsWaiting FROM {_waitingsTable} WHERE {_entityColumn} = @EntityId AND UserId = @UserId FOR UPDATE;",
            new { EntityId = entityId, UserId = userId },
            transaction);

        if (existingVote == isWaiting)
        {
            await connection.ExecuteAsync(
                $"DELETE FROM {_waitingsTable} WHERE {_entityColumn} = @EntityId AND UserId = @UserId;",
                new { EntityId = entityId, UserId = userId },
                transaction);
        }
        else
        {
            await connection.ExecuteAsync($@"INSERT INTO {_waitingsTable} ({_entityColumn}, UserId, IsWaiting)
VALUES (@EntityId, @UserId, @IsWaiting)
ON CONFLICT ({_entityColumn}, UserId) DO UPDATE SET IsWaiting = EXCLUDED.IsWaiting;",
                new { EntityId = entityId, UserId = userId, IsWaiting = isWaiting },
                transaction);
        }

        await transaction.CommitAsync();

        return (await GetStatisticsAsync(new[] { entityId }, userId)).First();
    }

    public async Task<WaitingStatistics> GetStatisticsAsync(long entityId, long? userId)
    {
        return (await GetStatisticsAsync(new[] { entityId }, userId)).First();
    }

    /// <summary>
    /// Статистика для набора Id. Для Id без голосов возвращаются нулевые счетчики.
    /// </summary>
    public async Task<IEnumerable<WaitingStatistics>> GetStatisticsAsync(long[] entitiesIds, long? userId)
    {
        if (entitiesIds.Length == 0)
            return Enumerable.Empty<WaitingStatistics>();

        using NpgsqlConnection connection = CreateConnection();

        IEnumerable<WaitingStatistics> statistics = await connection.QueryAsync<WaitingStatistics>($@"SELECT
ids.Id AS EntityId,
COUNT(w.Id) FILTER (WHERE w.IsWaiting)::int AS WaitingCount,
COUNT(w.Id) FILTER (WHERE NOT w.IsWaiting)::int AS NotWaitingCount,
BOOL_OR(w.IsWaiting) FILTER (WHERE w.UserId = @UserId) AS UserVote
FROM UNNEST(@EntitiesIds::bigint[]) AS ids(Id)
LEFT JOIN {_waitingsTable} w ON w.{_entityColumn} = ids.Id
GROUP BY ids.Id;",
            new { EntitiesIds = entitiesIds.Distinct().ToArray(), UserId = userId });

        return statistics;
    }
}
