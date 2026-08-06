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
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        long insertedGameReviewId = await connection.QueryFirstAsync<long>(@"
INSERT INTO GamesPlayersReviews (GameId, UserId, TextContent, Score, Date)
VALUES(@GameId, @UserId, @TextContent, @Score, @TimeStamp::DATE)
RETURNING Id;", new
        {
            gameReview.GameId,
            gameReview.UserId,
            gameReview.TextContent,
            gameReview.Score,
            gameReview.TimeStamp  // Pass DateTime, cast in SQL
        });

        return insertedGameReviewId;
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
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        IEnumerable<GameReview> gameReviewToCheckExistance = await connection.QueryAsync<GameReview, Game, GamePlayerReviewShift, ApplicationUser, GameReview>(@"
SELECT gpr.Id, gpr.GameId, gpr.UserId, gpr.TextContent, gpr.Score, gpr.Date,
Games.Id, Games.Name, Games.Image, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
gprs.Id, gprs.GamePlayerReviewId, gprs.ShifterId, gprs.Direction,
au.Id, au.UserName, au.NormalizedUserName, au.Email, au.NormalizedEmail, au.EmailConfirmed, au.PasswordHash, au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
FROM GamesPlayersReviews gpr
INNER JOIN Games
on gpr.GameId=Games.Id
INNER JOIN ApplicationUsers au
on gpr.UserId=au.Id
LEFT JOIN GamesPlayersReviewsShifts gprs on gprs.GamePlayerReviewId=gpr.Id
WHERE UserId=@userId and GameId=@gameId;", (gameReview, game, shift, applicationUser) =>
        {
            gameReview = gameReview with
            {
                Game = game,
                ApplicationUser = applicationUser
            };

            if (shift is not null && !gameReview.GamePlayerReviewShifts.Any(b => b.GamePlayerReviewId == shift.GamePlayerReviewId && b.ShifterId == shift.ShifterId))
            {
                gameReview.GamePlayerReviewShifts.Add(shift);
            }

            return gameReview;

        }, new { userId, gameId });

        return gameReviewToCheckExistance.FirstOrDefault();
    }

    public async Task<IEnumerable<GameReview>> GetAllAsync()
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        IEnumerable<GameReview> gamesReviews = await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
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

    public async Task<GameReview> GetAsync(long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        IEnumerable<GameReview> gamesReviews = await connection.QueryAsync<GameReview, GamePlayerReviewShift, Game, ApplicationUser, GameReview>(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
    gprs.Id, gprs.GamePlayerReviewId, gprs.ShifterId, gprs.Direction,
Games.Id, Games.Name, Games.Image, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
ApplicationUsers.Id, ApplicationUsers.UserName, ApplicationUsers.NormalizedUserName, ApplicationUsers.Email, ApplicationUsers.NormalizedEmail, ApplicationUsers.EmailConfirmed, ApplicationUsers.PasswordHash, ApplicationUsers.PhoneNumber, ApplicationUsers.PhoneNumberConfirmed, ApplicationUsers.TwoFactorEnabled
FROM GamesPlayersReviews
LEFT JOIN GamesPlayersReviewsShifts gprs on gprs.GamePlayerReviewId=GamesPlayersReviews.Id
INNER JOIN Games
on GamesPlayersReviews.GameId=Games.Id
INNER JOIN ApplicationUsers
on GamesPlayersReviews.UserId=ApplicationUsers.Id
WHERE GamesPlayersReviews.Id = @id;", (gameReview, gamePlayerReviewShift, game, applicationUser) =>
        {
            gameReview = gameReview with
            {
                Game = game,
                ApplicationUser = applicationUser
            };
            if (!gameReview.GamePlayerReviewShifts.Any(b => b.GamePlayerReviewId == gamePlayerReviewShift.GamePlayerReviewId && b.ShifterId == gamePlayerReviewShift.ShifterId))
                gameReview.GamePlayerReviewShifts.Add(gamePlayerReviewShift);
            return gameReview;
        }, new { id });

        return gamesReviews.SingleOrDefault();
    }

    public async Task<IEnumerable<GameReview>> GetAsync(long offset, long limit)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
ApplicationUsers.Id, ApplicationUsers.UserName, ApplicationUsers.NormalizedUserName, ApplicationUsers.Email, ApplicationUsers.NormalizedEmail, ApplicationUsers.EmailConfirmed, ApplicationUsers.PasswordHash, ApplicationUsers.PhoneNumber, ApplicationUsers.PhoneNumberConfirmed, ApplicationUsers.TwoFactorEnabled
FROM GamesPlayersReviews
INNER JOIN Games
on GamesPlayersReviews.GameId=Games.Id
INNER JOIN ApplicationUsers
on GamesPlayersReviews.UserId=ApplicationUsers.Id
ORDER BY GamesPlayersReviews.Id desc
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

    public async Task RemoveAsync(long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(@"DELETE FROM GamesPlayersReviews WHERE Id=@id", new { id });
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (long id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<GameReview> UpdateAsync(UpdateGamePlayerReviewModel gameReview, long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        GameReview? updatedGamePlayerReview = await connection.QueryFirstOrDefaultAsync<GameReview>(@"UPDATE GamesPlayersReviews 
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

    public async Task<IEnumerable<GameReview>> GetGameReviewsAsync(long gameId)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
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

    public async Task<IEnumerable<GameReview>> GetUserReviewsAsync(long userId)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
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
