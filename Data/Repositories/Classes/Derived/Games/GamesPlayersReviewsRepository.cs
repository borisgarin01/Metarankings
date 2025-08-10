using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.Reviews;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesPlayersReviewsRepository : Repository, IGamesReviewsRepository
{
    public GamesPlayersReviewsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(GameReview gameReview)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var insertedGameReviewId = await connection.QueryFirstAsync<long>(@"
INSERT INTO GamesPlayersReviews (GameId, UserId, TextContent, Score, Date)
OUTPUT inserted.Id
VALUES(@GameId, @UserId, @TextContent, @Score, @Date);", new
            {
                gameReview.GameId,
                gameReview.UserId,
                gameReview.TextContent,
                gameReview.Score,
                gameReview.Date
            });

            return insertedGameReviewId;
        }
    }

    public async Task AddRangeAsync(IEnumerable<GameReview> gamesReviews)
    {
        foreach (GameReview gameReview in gamesReviews)
        {
            await AddAsync(gameReview);
        }
    }

    public async Task<GameReview> GetUserReviewForGameAsync(long userId, long gameId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var gameReviewToCheckExistance = await connection.QueryFirstOrDefaultAsync<GameReview>(@"
SELECT Id, GameId, UserId, TextContent, Score, Date
FROM GamesPlayersReviews
WHERE UserId=@userId and GameId=@gameId;", new { userId, gameId });

            return gameReviewToCheckExistance;
        }
    }

    public async Task<IEnumerable<GameReview>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            return await connection.QueryAsync<GameReview>(@"SELECT Id, GameId, UserId, TextContent, Score, Date 
FROM GamesPlayersReviews;");
        }
    }

    public async Task<GameReview> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            return await connection.QueryFirstOrDefaultAsync<GameReview>(@"
SELECT Id, GameId, UserId, TextContent, Score, Date
FROM GamesPlayersReviews
WHERE Id = @id;", new { id });
        }
    }

    public async Task<IEnumerable<GameReview>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            return await connection.QueryAsync<GameReview>(@"SELECT Id, GameId, UserId, TextContent, Score, Date 
FROM GamesPlayersReviews
ORDER BY id
OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY", new { offset, limit });
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM GamesPlayersReviews WHERE Id=@id");
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<GameReview> UpdateAsync(GameReview gameReview, long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var updatedGamePlayerReview = await connection.QueryFirstOrDefaultAsync<GameReview>(@"UPDATE GamesPlayersReviews 
SET GameId=@GameId, UserId=@UserId, TextContent=@Text, Score=@Score, Date=@Date
WHERE Id=@id", new
            {
                gameReview.GameId,
                gameReview.UserId,
                gameReview.TextContent,
                gameReview.Score,
                gameReview.Date,
                id
            });

            return updatedGamePlayerReview;
        }
    }

    public async Task<IEnumerable<GameReview>> GetGameReviewsAsync(long gameId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            return await connection.QueryAsync<GameReview>(@"
SELECT Id, GameId, UserId, TextContent, Score, Date
FROM GamesPlayersReviews
WHERE GameId = @gameId;", new { gameId });
        }
    }

    public async Task<IEnumerable<GameReview>> GetUserReviewsAsync(long userId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            return await connection.QueryAsync<GameReview>(@"
SELECT Id, GameId, UserId, TextContent, Score, Date
FROM GamesPlayersReviews
WHERE UserId = @userId;", new { userId });
        }
    }
}
