using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.Reviews;
using IdentityLibrary.DTOs;

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
            var gameReviewToCheckExistance = await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.PublisherId, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
ApplicationUsers.Id, ApplicationUsers.UserName, ApplicationUsers.NormalizedUserName, ApplicationUsers.Email, ApplicationUsers.NormalizedEmail, ApplicationUsers.EmailConfirmed, ApplicationUsers.PasswordHash, ApplicationUsers.PhoneNumber, ApplicationUsers.PhoneNumberConfirmed, ApplicationUsers.TwoFactorEnabled
FROM GamesPlayersReviews
INNER JOIN Games
on GamesPlayersReviews.GameId=Games.Id
INNER JOIN ApplicationUsers
on GamesPlayersReviews.UserId=ApplicationUsers.Id
WHERE UserId=@userId and GameId=@gameId;", (gameReview, game, applicationUser) =>
            {
                gameReview = gameReview with
                {
                    Game = game,
                    ApplicationUser = applicationUser
                };

                return gameReview;

            }, new { userId, gameId });

            return gameReviewToCheckExistance.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<GameReview>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            IEnumerable<GameReview> gamesReviews = await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.PublisherId, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
ApplicationUsers.Id, ApplicationUsers.UserName, ApplicationUsers.NormalizedUserName, ApplicationUsers.Email, ApplicationUsers.NormalizedEmail, ApplicationUsers.EmailConfirmed, ApplicationUsers.PasswordHash, ApplicationUsers.PhoneNumber, ApplicationUsers.PhoneNumberConfirmed, ApplicationUsers.TwoFactorEnabled
FROM GamesPlayersReviews
INNER JOIN Games
on GamesPlayersReviews.GameId=Games.Id
INNER JOIN ApplicationUsers
on GamesPlayersReviews.UserId=ApplicationUsers.Id
WHERE UserId=@userId and GameId=@gameId;", (gameReview, game, applicationUser) =>
            {
                gameReview = gameReview with
                {
                    Game = game,
                    ApplicationUser = applicationUser
                };
                return gameReview;
            });

            return gamesReviews;
        }
    }

    public async Task<GameReview> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var gamesReviews = await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.PublisherId, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
ApplicationUsers.Id, ApplicationUsers.UserName, ApplicationUsers.NormalizedUserName, ApplicationUsers.Email, ApplicationUsers.NormalizedEmail, ApplicationUsers.EmailConfirmed, ApplicationUsers.PasswordHash, ApplicationUsers.PhoneNumber, ApplicationUsers.PhoneNumberConfirmed, ApplicationUsers.TwoFactorEnabled
FROM GamesPlayersReviews
INNER JOIN Games
on GamesPlayersReviews.GameId=Games.Id
INNER JOIN ApplicationUsers
on GamesPlayersReviews.UserId=ApplicationUsers.Id
WHERE GamesPlayersReviews.Id = @id;", (gameReview, game, applicationUser) =>
            {
                gameReview = gameReview with
                {
                    Game = game,
                    ApplicationUser = applicationUser
                };
                return gameReview;
            }, new { id });

            return gamesReviews.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<GameReview>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            return await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.PublisherId, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
ApplicationUsers.Id, ApplicationUsers.UserName, ApplicationUsers.NormalizedUserName, ApplicationUsers.Email, ApplicationUsers.NormalizedEmail, ApplicationUsers.EmailConfirmed, ApplicationUsers.PasswordHash, ApplicationUsers.PhoneNumber, ApplicationUsers.PhoneNumberConfirmed, ApplicationUsers.TwoFactorEnabled
FROM GamesPlayersReviews
INNER JOIN Games
on GamesPlayersReviews.GameId=Games.Id
INNER JOIN ApplicationUsers
on GamesPlayersReviews.UserId=ApplicationUsers.Id
ORDER BY GamesPlayersReviews.Id asc
OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY", (gameReview, game, applicationUser) =>
            {
                gameReview = gameReview with
                {
                    Game = game,
                    ApplicationUser = applicationUser
                };
                return gameReview;
            }, new { offset, limit });
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
            return await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.PublisherId, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
ApplicationUsers.Id, ApplicationUsers.UserName, ApplicationUsers.NormalizedUserName, ApplicationUsers.Email, ApplicationUsers.NormalizedEmail, ApplicationUsers.EmailConfirmed, ApplicationUsers.PasswordHash, ApplicationUsers.PhoneNumber, ApplicationUsers.PhoneNumberConfirmed, ApplicationUsers.TwoFactorEnabled
FROM GamesPlayersReviews
INNER JOIN Games
on GamesPlayersReviews.GameId=Games.Id
INNER JOIN ApplicationUsers
on GamesPlayersReviews.UserId=ApplicationUsers.Id
WHERE GameId = @gameId
ORDER BY GamesPlayersReviews.Id;", (gameReview, game, applicationUser) =>
            {
                gameReview = gameReview with
                {
                    Game = game,
                    ApplicationUser = applicationUser
                };
                return gameReview;
            }, new { gameId });
        }
    }

    public async Task<IEnumerable<GameReview>> GetUserReviewsAsync(long userId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            return await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.PublisherId, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
ApplicationUsers.Id, ApplicationUsers.UserName, ApplicationUsers.NormalizedUserName, ApplicationUsers.Email, ApplicationUsers.NormalizedEmail, ApplicationUsers.EmailConfirmed, ApplicationUsers.PasswordHash, ApplicationUsers.PhoneNumber, ApplicationUsers.PhoneNumberConfirmed, ApplicationUsers.TwoFactorEnabled
FROM GamesPlayersReviews
INNER JOIN Games
on GamesPlayersReviews.GameId=Games.Id
INNER JOIN ApplicationUsers
on GamesPlayersReviews.UserId=ApplicationUsers.Id
WHERE UserId = @userId;", (gameReview, game, applicationUser) =>
            {
                gameReview = gameReview with
                {
                    Game = game,
                    ApplicationUser = applicationUser
                };
                return gameReview;
            }, new { userId });
        }
    }
}
