using Domain.RequestsModels.CriticsReviews;
using Domain.Reviews;
using System.Text.RegularExpressions;

namespace Data.Repositories.Classes.Derived.CriticsReviews;

/// <summary>
/// Рецензии критиков (изданий) на игры и фильмы.
/// Имена таблицы и колонки подставляются в текст SQL (параметризовать идентификаторы в Postgres нельзя), поэтому:
/// наследовать класс можно только внутри сборки Data (private protected), а каждый идентификатор проверяется в конструкторе.
/// Все значения из запросов передаются только параметрами.
/// </summary>
public abstract class CriticsReviewsRepository : Repository
{
    private static readonly Regex SqlIdentifierRegex = new Regex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

    private readonly string _reviewsTable;
    private readonly string _entityColumn;

    private protected CriticsReviewsRepository(string connectionString, string reviewsTable, string entityColumn)
        : base(connectionString)
    {
        _reviewsTable = EnsureSqlIdentifier(reviewsTable, nameof(reviewsTable));
        _entityColumn = EnsureSqlIdentifier(entityColumn, nameof(entityColumn));
    }

    private static string EnsureSqlIdentifier(string identifier, string parameterName)
    {
        if (identifier is null || !SqlIdentifierRegex.IsMatch(identifier))
            throw new ArgumentException($"Недопустимый SQL-идентификатор: '{identifier}'", parameterName);

        return identifier;
    }

    private string SelectSql => $@"
SELECT Id, {_entityColumn} AS EntityId, Publication, Author, Score, TextContent, SourceUrl, Date
FROM {_reviewsTable}";

    /// <summary>
    /// Рецензии на игру/фильм: сначала высокие оценки.
    /// </summary>
    public async Task<IEnumerable<CriticReview>> GetByEntityAsync(long entityId)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<CriticReview>(
            $"{SelectSql} WHERE {_entityColumn} = @EntityId ORDER BY Score DESC, Date DESC, Id;",
            new { EntityId = entityId });
    }

    public async Task<CriticReview?> GetAsync(long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryFirstOrDefaultAsync<CriticReview>($"{SelectSql} WHERE Id = @Id;", new { Id = id });
    }

    /// <param name="userId">Администратор, добавивший рецензию.</param>
    public async Task<long> AddAsync(CriticReviewModel model, long userId)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryFirstAsync<long>($@"
INSERT INTO {_reviewsTable} ({_entityColumn}, UserId, Publication, Author, Score, TextContent, SourceUrl, Date)
VALUES (@EntityId, @UserId, @Publication, @Author, @Score, @TextContent, @SourceUrl, @Date)
RETURNING Id;",
            new
            {
                model.EntityId,
                UserId = userId,
                Publication = model.Publication.Trim(),
                Author = NullIfEmpty(model.Author),
                model.Score,
                TextContent = NullIfEmpty(model.TextContent),
                SourceUrl = NullIfEmpty(model.SourceUrl),
                Date = ToDbDate(model.Date)
            });
    }

    public async Task<CriticReview?> UpdateAsync(CriticReviewModel model, long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryFirstOrDefaultAsync<CriticReview>($@"
UPDATE {_reviewsTable}
SET Publication = @Publication, Author = @Author, Score = @Score, TextContent = @TextContent, SourceUrl = @SourceUrl, Date = @Date
WHERE Id = @Id
RETURNING Id, {_entityColumn} AS EntityId, Publication, Author, Score, TextContent, SourceUrl, Date;",
            new
            {
                Id = id,
                Publication = model.Publication.Trim(),
                Author = NullIfEmpty(model.Author),
                model.Score,
                TextContent = NullIfEmpty(model.TextContent),
                SourceUrl = NullIfEmpty(model.SourceUrl),
                Date = ToDbDate(model.Date)
            });
    }

    public async Task RemoveAsync(long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync($"DELETE FROM {_reviewsTable} WHERE Id = @Id;", new { Id = id });
    }

    // Dapper не умеет передавать DateOnly параметром, поэтому дата уходит как DateTime и приводится к date в Postgres
    private static DateTime ToDbDate(DateOnly date) => date.ToDateTime(TimeOnly.MinValue);

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
