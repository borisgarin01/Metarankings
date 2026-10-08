using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.Reviews;
using IdentityLibrary.DTOs;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesPlayersReviewsRepository : Repository<GameReview, AddGamePlayerReviewWithUserIdAndDateModel, UpdateGamePlayerReviewModel>, IGamesPlayersReviewsRepository
{
    public GamesPlayersReviewsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddGamePlayerReviewWithUserIdAndDateModel gameReview, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        long insertedGameReviewId = await connection.QueryFirstAsync<long>(new CommandDefinition(@"
INSERT INTO GamesPlayersReviews (GameId, UserId, TextContent, Score, Date)
VALUES(@GameId, @UserId, @TextContent, @Score, @TimeStamp::DATE)
RETURNING Id;", new
        {
            gameReview.GameId,
            gameReview.UserId,
            gameReview.TextContent,
            gameReview.Score,
            gameReview.TimeStamp  // Pass DateTime, cast in SQL
        }, cancellationToken: cancellationToken));

        return insertedGameReviewId;
    }

    public async Task<GameReview> GetUserReviewForGameAsync(long userId, long gameId, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        IEnumerable<GameReview> gameReviewToCheckExistance = await connection.QueryAsync<GameReview, Game, GamePlayerReviewShift, ApplicationUser, GameReview>(new CommandDefinition(@"
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
WHERE UserId=@userId and GameId=@gameId;", new { userId, gameId }, cancellationToken: cancellationToken), (gameReview, game, shift, applicationUser) =>
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

        });

        return gameReviewToCheckExistance.FirstOrDefault();
    }

    public override async Task<IEnumerable<GameReview>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        IEnumerable<GameReview> gamesReviews = await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(new CommandDefinition(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
ApplicationUsers.Id, ApplicationUsers.UserName, ApplicationUsers.NormalizedUserName, ApplicationUsers.Email, ApplicationUsers.NormalizedEmail, ApplicationUsers.EmailConfirmed, ApplicationUsers.PasswordHash, ApplicationUsers.PhoneNumber, ApplicationUsers.PhoneNumberConfirmed, ApplicationUsers.TwoFactorEnabled
FROM GamesPlayersReviews
INNER JOIN Games
on GamesPlayersReviews.GameId=Games.Id
INNER JOIN ApplicationUsers
on GamesPlayersReviews.UserId=ApplicationUsers.Id
WHERE UserId=@userId and GameId=@gameId;", cancellationToken: cancellationToken), (gameReview, game, applicationUser) =>
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

    public override async Task<GameReview> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        IEnumerable<GameReview> gamesReviews = await connection.QueryAsync<GameReview, GamePlayerReviewShift, Game, ApplicationUser, GameReview>(new CommandDefinition(@"
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
WHERE GamesPlayersReviews.Id = @id;", new { id }, cancellationToken: cancellationToken), (gameReview, gamePlayerReviewShift, game, applicationUser) =>
        {
            gameReview = gameReview with
            {
                Game = game,
                ApplicationUser = applicationUser
            };
            if (!gameReview.GamePlayerReviewShifts.Any(b => b.GamePlayerReviewId == gamePlayerReviewShift.GamePlayerReviewId && b.ShifterId == gamePlayerReviewShift.ShifterId))
                gameReview.GamePlayerReviewShifts.Add(gamePlayerReviewShift);
            return gameReview;
        });

        return gamesReviews.SingleOrDefault();
    }

    public override async Task<IEnumerable<GameReview>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(new CommandDefinition(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
ApplicationUsers.Id, ApplicationUsers.UserName, ApplicationUsers.NormalizedUserName, ApplicationUsers.Email, ApplicationUsers.NormalizedEmail, ApplicationUsers.EmailConfirmed, ApplicationUsers.PasswordHash, ApplicationUsers.PhoneNumber, ApplicationUsers.PhoneNumberConfirmed, ApplicationUsers.TwoFactorEnabled
FROM GamesPlayersReviews
INNER JOIN Games
on GamesPlayersReviews.GameId=Games.Id
INNER JOIN ApplicationUsers
on GamesPlayersReviews.UserId=ApplicationUsers.Id
ORDER BY GamesPlayersReviews.Id desc
OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY", new { offset, limit }, cancellationToken: cancellationToken), (gameReview, game, applicationUser) =>
        {
            gameReview = gameReview with
            {
                Game = game,
                ApplicationUser = applicationUser
            };
            return gameReview;
        });
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM GamesPlayersReviews WHERE Id=@id", new { id }, cancellationToken: cancellationToken));
    }

    public override async Task<GameReview> UpdateAsync(UpdateGamePlayerReviewModel gameReview, long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        GameReview? updatedGamePlayerReview = await connection.QueryFirstOrDefaultAsync<GameReview>(new CommandDefinition(@"UPDATE GamesPlayersReviews 
SET TextContent=@TextContent, Score=@Score, Date=@TimeStamp
WHERE Id=@id", new
        {
            gameReview.TextContent,
            gameReview.Score,
            TimeStamp = DateTime.Now,
            id
        }, cancellationToken: cancellationToken));

        return updatedGamePlayerReview;
    }

    public async Task<IEnumerable<GameReview>> GetGameReviewsAsync(long gameId, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(new CommandDefinition(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
ApplicationUsers.Id, ApplicationUsers.UserName, ApplicationUsers.NormalizedUserName, ApplicationUsers.Email, ApplicationUsers.NormalizedEmail, ApplicationUsers.EmailConfirmed, ApplicationUsers.PasswordHash, ApplicationUsers.PhoneNumber, ApplicationUsers.PhoneNumberConfirmed, ApplicationUsers.TwoFactorEnabled
FROM GamesPlayersReviews
INNER JOIN Games
on GamesPlayersReviews.GameId=Games.Id
INNER JOIN ApplicationUsers
on GamesPlayersReviews.UserId=ApplicationUsers.Id
WHERE GameId = @gameId
ORDER BY GamesPlayersReviews.Id;", new { gameId }, cancellationToken: cancellationToken), (gameReview, game, applicationUser) =>
        {
            gameReview = gameReview with
            {
                Game = game,
                ApplicationUser = applicationUser
            };
            return gameReview;
        });
    }

    public async Task<IEnumerable<GameReview>> GetUserReviewsAsync(long userId, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<GameReview, Game, ApplicationUser, GameReview>(new CommandDefinition(@"
SELECT GamesPlayersReviews.Id, GamesPlayersReviews.GameId, GamesPlayersReviews.UserId, GamesPlayersReviews.TextContent, GamesPlayersReviews.Score, GamesPlayersReviews.Date,
Games.Id, Games.Name, Games.Image, Games.ReleaseDate, Games.Description, Games.Trailer, Games.LocalizationId,
ApplicationUsers.Id, ApplicationUsers.UserName, ApplicationUsers.NormalizedUserName, ApplicationUsers.Email, ApplicationUsers.NormalizedEmail, ApplicationUsers.EmailConfirmed, ApplicationUsers.PasswordHash, ApplicationUsers.PhoneNumber, ApplicationUsers.PhoneNumberConfirmed, ApplicationUsers.TwoFactorEnabled
FROM GamesPlayersReviews
INNER JOIN Games
on GamesPlayersReviews.GameId=Games.Id
INNER JOIN ApplicationUsers
on GamesPlayersReviews.UserId=ApplicationUsers.Id
WHERE UserId = @userId;", new { userId }, cancellationToken: cancellationToken), (gameReview, game, applicationUser) =>
        {
            gameReview = gameReview with
            {
                Game = game,
                ApplicationUser = applicationUser
            };
            return gameReview;
        });
    }
}
