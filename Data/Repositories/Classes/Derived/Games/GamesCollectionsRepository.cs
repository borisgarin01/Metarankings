using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using Domain.Reviews;
using IdentityLibrary.DTOs;
using Npgsql;
using Dapper;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesCollectionsRepository : Repository<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel>
{
    public GamesCollectionsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddGamesCollectionModel entity, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        const string sql = @"
            INSERT INTO GamesCollections(Name, Description, ImageSource) 
            VALUES(@Name, @Description, @ImageSource)
            RETURNING Id;";

        return await connection.QuerySingleAsync<long>(new CommandDefinition(sql, new
        {
            entity.Name,
            entity.Description,
            entity.ImageSource
        }, cancellationToken: cancellationToken));
    }

    public override async Task AddRangeAsync(IEnumerable<AddGamesCollectionModel> entities, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        const string sql = @"
            INSERT INTO GamesCollections(Name, Description, ImageSource) 
            VALUES(@Name, @Description, @ImageSource)";

        // Dapper выполняет команду для каждого элемента отдельно и без транзакции,
        // поэтому отмена учитывается только до начала вставки
        cancellationToken.ThrowIfCancellationRequested();
        await connection.ExecuteAsync(new CommandDefinition(sql, entities, cancellationToken: CancellationToken.None));
    }

    public override async Task<IEnumerable<GamesCollection>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        var gamesCollections = await connection.QueryAsync<GamesCollection, GamesCollectionItem, Game, GameReview, ApplicationUser, GamesCollection>(
            new CommandDefinition(@"
            SELECT gc.Id, gc.Name, gc.Description, gc.ImageSource,
                   gci.Id, gci.GameId, gci.GameCollectionId,
                   g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer,
                   COALESCE((SELECT AVG(Score)::float FROM GamesPlayersReviews WHERE GameId = g.Id), 0) AS Score,
                   COALESCE((SELECT COUNT(*) FROM GamesPlayersReviews WHERE GameId = g.Id), 0) AS ScoresCount,
                   gpr.Id, gpr.GameId, gpr.UserId, gpr.Score, gpr.TextContent, gpr.Date,
                   au.Id, au.UserName, au.NormalizedUserName, au.EmailConfirmed, au.PasswordHash, 
                   au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
            FROM GamesCollections gc
            LEFT JOIN GamesCollectionsItems gci ON gc.Id = gci.GameCollectionId
            LEFT JOIN Games g ON g.Id = gci.GameId
            LEFT JOIN GamesPlayersReviews gpr ON gpr.GameId = g.Id
            LEFT JOIN ApplicationUsers au ON au.Id = gpr.UserId
            ORDER BY gc.Id", cancellationToken: cancellationToken),
            (gameCollection, gameCollectionItem, game, gamePlayerReview, applicationUser) =>
            {
                if (game is not null && gameCollectionItem is not null)
                {
                    if (!gameCollection.GamesCollectionItems.Any(g => g.GameId == game.Id))
                    {
                        gameCollectionItem.Game = game;
                        gameCollectionItem.GameId = game.Id;
                        gameCollectionItem.GamesCollection = gameCollection;
                        gameCollectionItem.GamesCollectionId = gameCollection.Id;
                        gameCollection.GamesCollectionItems.Add(gameCollectionItem);
                    }

                    if (gamePlayerReview is not null && applicationUser is not null)
                    {
                        if (!game.GamesPlayersReviews.Any(b => b.UserId == gamePlayerReview.UserId))
                        {
                            gamePlayerReview.Game = game;
                            gamePlayerReview.GameId = game.Id;
                            gamePlayerReview.ApplicationUser = applicationUser;
                            gamePlayerReview.UserId = applicationUser.Id;
                            game.GamesPlayersReviews.Add(gamePlayerReview);
                        }
                    }
                }

                return gameCollection;
            },
            splitOn: "Id");

        return gamesCollections.DistinctBy(gc => gc.Id);
    }

    public override async Task<GamesCollection?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        var gamesCollection = await connection.QueryAsync<GamesCollection, GamesCollectionItem, Game, GameReview, ApplicationUser, GamesCollection>(
            new CommandDefinition(@"
        SELECT gc.Id, gc.Name, gc.Description, gc.ImageSource,
               gci.Id, gci.GameId, gci.GameCollectionId,
               g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer,
               COALESCE((SELECT AVG(Score)::float FROM GamesPlayersReviews WHERE GameId = g.Id), 0) AS UsersScore,
               COALESCE((SELECT COUNT(*) FROM GamesPlayersReviews WHERE GameId = g.Id), 0) AS UsersReviewsCount,
               gpr.Id, gpr.GameId, gpr.UserId, gpr.Score, gpr.TextContent, gpr.Date,
               au.Id, au.UserName, au.NormalizedUserName, au.EmailConfirmed, au.PasswordHash, 
               au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
        FROM GamesCollections gc
        LEFT JOIN GamesCollectionsItems gci ON gc.Id = gci.GameCollectionId
        LEFT JOIN Games g ON g.Id = gci.GameId
        LEFT JOIN GamesPlayersReviews gpr ON gpr.GameId = g.Id
        LEFT JOIN ApplicationUsers au ON au.Id = gpr.UserId
        WHERE gc.Id = @Id", new { Id = id }, cancellationToken: cancellationToken),
            (gameCollection, gameCollectionItem, game, gamePlayerReview, applicationUser) =>
            {
                if (gameCollection is null)
                    return null;

                // Добавляем игру в коллекцию
                if (game is not null && gameCollectionItem is not null)
                {
                    if (!gameCollection.GamesCollectionItems.Any(g => g.GameId == game.Id))
                    {
                        gameCollectionItem.Game = game;
                        gameCollectionItem.GameId = game.Id;
                        gameCollectionItem.GamesCollection = gameCollection;
                        gameCollectionItem.GamesCollectionId = gameCollection.Id;
                        gameCollection.GamesCollectionItems.Add(gameCollectionItem);
                    }
                }

                // Добавляем отзыв к игре
                if (game is not null && gamePlayerReview is not null && applicationUser is not null)
                {
                    var existingGame = gameCollection.GamesCollectionItems
                        .FirstOrDefault(g => g.GameId == game.Id)?.Game;

                    if (existingGame is not null &&
                        !existingGame.GamesPlayersReviews.Any(b => b.UserId == gamePlayerReview.UserId))
                    {
                        gamePlayerReview.Game = existingGame;
                        gamePlayerReview.GameId = existingGame.Id;
                        gamePlayerReview.ApplicationUser = applicationUser;
                        gamePlayerReview.UserId = applicationUser.Id;
                        existingGame.GamesPlayersReviews.Add(gamePlayerReview);
                    }
                }

                return gameCollection;
            },
            splitOn: "Id,Id,Id,Id");

        // Группировка результатов
        GamesCollection? result = gamesCollection
            .Where(gc => gc is not null)
            .GroupBy(gc => gc.Id)
            .Select(g =>
            {
                GamesCollection groupedCollection = g.First();
                groupedCollection.GamesCollectionItems = g
                    .SelectMany(gc => gc.GamesCollectionItems)
                    .DistinctBy(item => item.GameId)
                    .ToList();
                return groupedCollection;
            })
            .SingleOrDefault();

        return result;
    }

    public override async Task<IEnumerable<GamesCollection>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        var gamesCollectionsDictionary = new Dictionary<long, GamesCollection>();

        await connection.QueryAsync<GamesCollection, GamesCollectionItem, Game, GameReview, ApplicationUser, GamesCollection>(
            new CommandDefinition(@"
            SELECT gc.Id, gc.Name, gc.Description, gc.ImageSource,
                   gci.Id, gci.GameId, gci.GameCollectionId,
                   g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer,
                   COALESCE((SELECT AVG(Score)::float FROM GamesPlayersReviews WHERE GameId = g.Id), 0) AS Score,
                   COALESCE((SELECT COUNT(*) FROM GamesPlayersReviews WHERE GameId = g.Id), 0) AS ScoresCount,
                   gpr.Id, gpr.GameId, gpr.UserId, gpr.Score, gpr.TextContent, gpr.Date,
                   au.Id, au.UserName, au.NormalizedUserName, au.EmailConfirmed, au.PasswordHash, 
                   au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
            FROM GamesCollections gc
            LEFT JOIN GamesCollectionsItems gci ON gc.Id = gci.GameCollectionId
            LEFT JOIN Games g ON g.Id = gci.GameId
            LEFT JOIN GamesPlayersReviews gpr ON gpr.GameId = g.Id
            LEFT JOIN ApplicationUsers au ON au.Id = gpr.UserId
            WHERE gc.Id IN (
                SELECT Id 
                FROM GamesCollections 
                ORDER BY Id ASC 
                OFFSET @offset LIMIT @limit
            )
            ORDER BY gc.Id", new { offset, limit }, cancellationToken: cancellationToken),
            (gameCollection, gameCollectionItem, game, gamePlayerReview, applicationUser) =>
            {
                if (!gamesCollectionsDictionary.TryGetValue(gameCollection.Id, out var existingCollection))
                {
                    gamesCollectionsDictionary.Add(gameCollection.Id, gameCollection);
                    existingCollection = gameCollection;
                }

                if (gameCollectionItem is not null && game is not null)
                {
                    if (!existingCollection.GamesCollectionItems.Any(g => g.GameId == game.Id))
                    {
                        gameCollectionItem.Game = game;
                        existingCollection.GamesCollectionItems.Add(gameCollectionItem);
                    }

                    if (gamePlayerReview is not null && applicationUser is not null)
                    {
                        if (!game.GamesPlayersReviews.Any(b => b.UserId == gamePlayerReview.UserId))
                        {
                            gamePlayerReview.Game = game;
                            gamePlayerReview.GameId = game.Id;
                            gamePlayerReview.ApplicationUser = applicationUser;
                            gamePlayerReview.UserId = applicationUser.Id;
                            game.GamesPlayersReviews.Add(gamePlayerReview);
                        }
                    }
                }

                return existingCollection;
            },
            splitOn: "Id");

        return gamesCollectionsDictionary.Values;
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition("DELETE FROM GamesCollections WHERE Id = @Id", new { Id = id }, cancellationToken: cancellationToken));
    }

    public override async Task RemoveRangeAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition("DELETE FROM GamesCollections WHERE Id = ANY(@Ids)", new { Ids = ids }, cancellationToken: cancellationToken));
    }

    public override async Task<GamesCollection> UpdateAsync(UpdateGamesCollectionModel entity, long id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        const string sql = @"
            UPDATE GamesCollections 
            SET Name = @Name, 
                ImageSource = @ImageSource 
            WHERE Id = @Id
            RETURNING Id, Name, Description, ImageSource;";

        return await connection.QuerySingleOrDefaultAsync<GamesCollection>(new CommandDefinition(sql, new
        {
            Name = entity.CollectionName,
            ImageSource = entity.ImageSource,
            Id = id
        }, cancellationToken: cancellationToken));
    }
}