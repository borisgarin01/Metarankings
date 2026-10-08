using Domain.ContentRequests;
using Domain.RequestsModels.ContentRequests;

namespace Data.Repositories.Classes.Derived.ContentRequests;

/// <summary>
/// Заявки пользователей на добавление игр и фильмов.
/// </summary>
public sealed class ContentRequestsRepository : Repository
{
    private const string SelectSql = @"SELECT
r.Id, r.UserId, u.UserName, r.ContentType, r.Title, r.Description,
r.Status, r.AdminComment, r.CreatedTimestamp, r.ProcessedTimestamp
FROM ContentRequests r
LEFT JOIN ApplicationUsers u ON u.Id = r.UserId";

    public ContentRequestsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddContentRequestModel addContentRequestModel, long userId, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstAsync<long>(new CommandDefinition(@"INSERT INTO ContentRequests
(UserId, ContentType, Title, Description)
VALUES (@UserId, @ContentType, @Title, @Description)
RETURNING Id;", new
            {
                UserId = userId,
                ContentType = (short)addContentRequestModel.ContentType!.Value,
                Title = addContentRequestModel.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(addContentRequestModel.Description) ? null : addContentRequestModel.Description.Trim()
            }, cancellationToken: cancellationToken));
    }

    public async Task<ContentRequest?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ContentRequest>(new CommandDefinition($"{SelectSql} WHERE r.Id = @Id;", new { Id = id }, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Все заявки (для администраторов), опционально только с указанным статусом. Сначала новые.
    /// </summary>
    public async Task<IEnumerable<ContentRequest>> GetAllAsync(ContentRequestStatus? status, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<ContentRequest>(
            new CommandDefinition($"{SelectSql} WHERE (@Status::smallint IS NULL OR r.Status = @Status) ORDER BY r.CreatedTimestamp DESC;", new { Status = (short?)status }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<ContentRequest>> GetByUserAsync(long userId, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<ContentRequest>(
            new CommandDefinition($"{SelectSql} WHERE r.UserId = @UserId ORDER BY r.CreatedTimestamp DESC;", new { UserId = userId }, cancellationToken: cancellationToken));
    }

    public async Task<int> CountPendingByUserAsync(long userId, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("SELECT COUNT(*)::int FROM ContentRequests WHERE UserId = @UserId AND Status = @Status;", new { UserId = userId, Status = (short)ContentRequestStatus.Pending }, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Меняет статус заявки. null - заявки с таким Id нет.
    /// </summary>
    public async Task<ContentRequest?> UpdateStatusAsync(long id, UpdateContentRequestStatusModel updateContentRequestStatusModel, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        int affected = await connection.ExecuteAsync(new CommandDefinition(@"UPDATE ContentRequests
SET Status = @Status,
AdminComment = @AdminComment,
ProcessedTimestamp = CASE WHEN @Status = 0 THEN NULL ELSE NOW() END
WHERE Id = @Id;", new
            {
                Id = id,
                Status = (short)updateContentRequestStatusModel.Status,
                AdminComment = string.IsNullOrWhiteSpace(updateContentRequestStatusModel.AdminComment) ? null : updateContentRequestStatusModel.AdminComment.Trim()
            }, cancellationToken: cancellationToken));

        return affected == 0 ? null : await GetAsync(id, cancellationToken);
    }

    public async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition("DELETE FROM ContentRequests WHERE Id = @Id;", new { Id = id }, cancellationToken: cancellationToken));
    }
}
