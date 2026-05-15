using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using Domain.Reviews;
using IdentityLibrary.DTOs;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesCollectionsRepository : Repository, IRepository<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel>
{
    public GamesCollectionsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddGamesCollectionModel entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var insertedGameCollectionId = await connection.QuerySingleAsync<long>(@"
INSERT INTO GamesCollections(Name,Description,ImageSource) 
VALUES(@Name, @Description, @ImageSource)
RETURNING Id;", new { entity.Name, entity.Description, entity.ImageSource });

            return insertedGameCollectionId;
        }
    }

    public async Task AddRangeAsync(IEnumerable<AddGamesCollectionModel> entities)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity);
        }
    }

    public async Task<IEnumerable<GamesCollection>> GetAllAsync()
    {
        using var connection = new NpgsqlConnection(ConnectionString);

        var gamesCollections = await connection.QueryAsync<GamesCollection, GamesCollectionItem, Game, GameReview, ApplicationUser, GamesCollection>(
            @"SELECT gc.Id, gc.Name, gc.Description, gc.ImageSource,
                gci.Id, gci.GameId, gci.GameCollectionId,
                 g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer,
                gpr.Id, gpr.GameId, gpr.UserId, gpr.Score, gpr.TextContent, gpr.Date,
                au.Id, au.UserName, au.NormalizedUserName, au.EmailConfirmed, au.PasswordHash, au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
          FROM GamesCollections gc
          LEFT JOIN GamesCollectionsItems gci ON gc.Id = gci.GameCollectionId
          LEFT JOIN Games g ON g.Id = gci.GameId
          LEFT JOIN GamesPlayersReviews gpr on gpr.GameId=g.Id
          LEFT JOIN ApplicationUsers au on au.Id = gpr.UserId
          ORDER BY gc.Id",
            (gameCollection, gameCollectionItem, game, gamePlayerReview, applicationUser) =>
            {
                if (game is not null && gameCollectionItem is not null && !gameCollection.GamesCollectionItems.Any(g => g.GameId == game.Id))
                {
                    gameCollectionItem.Game = game;
                    gameCollectionItem.GameId = game.Id;
                    gameCollectionItem.GamesCollection = gameCollection;
                    gameCollectionItem.GamesCollectionId = gameCollection.Id;
                    gameCollection.GamesCollectionItems.Add(gameCollectionItem);

                    if (gamePlayerReview is not null && applicationUser is not null && !game.GamesPlayersReviews.Any(b => b.UserId == gamePlayerReview.UserId))
                    {
                        gamePlayerReview.Game = game;
                        gamePlayerReview.GameId = game.Id;
                        gamePlayerReview.ApplicationUser = applicationUser;
                        gamePlayerReview.UserId = applicationUser.Id;
                        game.GamesPlayersReviews.Add(gamePlayerReview);
                    }
                }

                return gameCollection;
            },
            splitOn: "Id");

        return gamesCollections;
    }

    public async Task<GamesCollection?> GetAsync(long id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);

        var gamesCollection = await connection.QueryAsync<GamesCollection, GamesCollectionItem, Game, GameReview, ApplicationUser, GamesCollection>(
            @"SELECT gc.Id, gc.Name, gc.Description, gc.ImageSource,
                gci.Id, gci.GameId, gci.GameCollectionId,
                 g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer,
                gpr.Id, gpr.GameId, gpr.UserId, gpr.Score, gpr.TextContent, gpr.Date,
                au.Id, au.UserName, au.NormalizedUserName, au.EmailConfirmed, au.PasswordHash, au.PhoneNumber
          FROM GamesCollections gc
          LEFT JOIN GamesCollectionsItems gci ON gc.Id = gci.GameCollectionId
          LEFT JOIN Games g ON g.Id = gci.GameId
          LEFT JOIN GamesPlayersReviews gpr on gpr.GameId=g.Id
          LEFT JOIN ApplicationUsers au on au.Id = gpr.UserId
          WHERE gc.Id = @Id",
            (gameCollection, gameCollectionItem, game, gamePlayerReview, applicationUser) =>
            {
                if (game is not null && gameCollectionItem is not null && !gameCollection.GamesCollectionItems.Any(g => g.GameId == game.Id))
                {
                    gameCollectionItem.Game = game;
                    gameCollectionItem.GameId = game.Id;
                    gameCollectionItem.GamesCollection = gameCollection;
                    gameCollectionItem.GamesCollectionId = gameCollection.Id;

                    if (gamePlayerReview is not null && applicationUser is not null && !game.GamesPlayersReviews.Any(b => b.UserId == gamePlayerReview.UserId))
                    {
                        gamePlayerReview.Game = game;
                        gamePlayerReview.GameId = game.Id;
                        gamePlayerReview.ApplicationUser = applicationUser;
                        gamePlayerReview.UserId = applicationUser.Id;
                        game.GamesPlayersReviews.Add(gamePlayerReview);
                    }
                    gameCollection.GamesCollectionItems.Add(gameCollectionItem);
                }

                return gameCollection;
            },
            new { Id = id },
            splitOn: "Id");


        IEnumerable<GamesCollection> gamesCollectionGrouped = gamesCollection.GroupBy(b => new { b.Id })
                .Select(g =>
                {
                    GamesCollection gameCollection = g.First();
                    gameCollection.GamesCollectionItems = g.SelectMany(b => b.GamesCollectionItems).ToList();
                    return gameCollection;
                });

        return gamesCollectionGrouped.SingleOrDefault();
    }

    public async Task<IEnumerable<GamesCollection>> GetAsync(long offset, long limit)
    {
        using var connection = new NpgsqlConnection(ConnectionString);

        var gamesCollectionsDictionary = new Dictionary<long, GamesCollection>();

        await connection.QueryAsync<GamesCollection, GamesCollectionItem, Game, GamesCollection>(
            @"SELECT gc.Id, gc.Name, gc.Description, gc.ImageSource,
                 gci.Id, gci.GameId, gci.GameCollectionId,
                 g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer
          FROM GamesCollections gc
          LEFT JOIN GamesCollectionsItems gci ON gc.Id = gci.GameCollectionId
          LEFT JOIN Games g ON g.Id = gci.GameId
          WHERE gc.Id IN (
              SELECT Id 
              FROM GamesCollections 
              ORDER BY Id ASC 
              OFFSET @offset LIMIT @limit
          )
          ORDER BY gc.Id",
            (gameCollection, gameCollectionItem, game) =>
            {
                if (!gamesCollectionsDictionary.TryGetValue(gameCollection.Id, out var existingCollection))
                {
                    gamesCollectionsDictionary.Add(gameCollection.Id, gameCollection);
                    existingCollection = gameCollection;
                }

                if (gameCollectionItem is not null && game is not null)
                {
                    gameCollectionItem.Game = game;
                    existingCollection.GamesCollectionItems.Add(gameCollectionItem);
                }

                return existingCollection;
            },
            new { offset, limit },
            splitOn: "Id,Id");

        return gamesCollectionsDictionary.Values;
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM GamesCollections WHERE Id=@Id", new { Id = id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<GamesCollection> UpdateAsync(UpdateGamesCollectionModel entity, long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var updatedGameCollection = await connection.QuerySingleOrDefaultAsync<GamesCollection>(@"UPDATE GamesCollections 
SET Name=@Name, ImageSource=@ImageSource 
WHERE Id=@Id
RETURNING Name, Id;", new
            {
                Name = entity.CollectionName,
                ImageSource = entity.ImageSource,
                Id = id
            });

            return updatedGameCollection;
        }
    }
}
