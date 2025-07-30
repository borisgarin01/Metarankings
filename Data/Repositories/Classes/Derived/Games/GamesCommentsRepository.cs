using Data.Repositories.Interfaces;
using Domain.Games;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesCommentsRepository : Repository, IRepository<GameComment>
{
    public GamesCommentsRepository(string connectionString) : base(connectionString)
    {
    }

    public Task<long> AddAsync(GameComment entity)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            return connection.QueryFirstAsync<long>(@"
                INSERT INTO GameComments (GameId, UserId, TextContent)
                OUTPUT inserted.Id
                VALUES (@GameId, @UserId, @TextContent);",
                new
                {
                    entity.GameId,
                    entity.UserId,
                    entity.TextContent
                });
        }
    }

    public async Task AddRangeAsync(IEnumerable<GameComment> entities)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity);
        }
    }

    public async Task<IEnumerable<GameComment>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var gamesComments = await connection.QueryAsync<GameComment>(@"
                SELECT Id, GameId, UserId, TextContent
                FROM GameComments;");

            return gamesComments;
        }
    }

    public async Task<GameComment> GetAsync(long id)
    {
        using(var connection=new SqlConnection(ConnectionString))
        {

            var gamesComment = await connection.QueryFirstOrDefaultAsync<GameComment>(@"
                SELECT Id, GameId, UserId, TextContent
                FROM GameComments
                WHERE Id=@id;", new { id });

            return gamesComment;
        }
    }

    public Task<IEnumerable<GameComment>> GetAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        throw new NotImplementedException();
    }

    public Task<GameComment> UpdateAsync(GameComment entity, long id)
    {
        throw new NotImplementedException();
    }
}
