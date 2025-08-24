using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.Reviews;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesPlayersReviewsRepository : Repository, IGamesPlayersReviewsRepository
{
    public GamesPlayersReviewsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddGamePlayerReviewWithUserIdAndDateModel gameReview)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var insertedGameReviewId = await connection.QueryFirstAsync<long>(@"
INSERT INTO GamesPlayersReviews (GameId, UserId, TextContent, Score, Date)
OUTPUT inserted.Id
VALUES(@GameId, @UserId, @TextContent, @Score, @TimeStamp);", new
            {
                gameReview.GameId,
                gameReview.UserId,
                gameReview.TextContent,
                gameReview.Score,
                gameReview.TimeStamp
            });

            return insertedGameReviewId;
        }
    }

    public async Task AddRangeAsync(IEnumerable<AddGamePlayerReviewWithUserIdAndDateModel> gamesReviews)
    {
        foreach (AddGamePlayerReviewWithUserIdAndDateModel gameReview in gamesReviews)
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
            await connection.ExecuteAsync(@"DELETE FROM GamesPlayersReviews WHERE Id=@id", new { id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<GameReview> UpdateAsync(UpdateGamePlayerReviewModel gameReview, long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var updatedGamePlayerReview = await connection.QueryFirstOrDefaultAsync<GameReview>(@"UPDATE GamesPlayersReviews 
SET TextContent=@TextContent, Score=@Score, Date=@TimeStamp
WHERE Id=@id", new
            {
                gameReview.TextContent,
                gameReview.Score,
                TimeStamp = DateTime.Now,
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
