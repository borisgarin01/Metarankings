using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesCollectionsItemsRepository(string connectionString) : Repository<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel>(connectionString)
{
    public override async Task<long> AddAsync(AddGamesCollectionItemModel entity)
    {
        using (var connection = CreateConnection())
        {
            var insertedGameCollectionItemId = await connection.QuerySingleAsync<long>(@"
INSERT INTO GamesCollectionsItems (GameId, GameCollectionId)
VALUES (@GameId, @GameCollectionId)
RETURNING Id;",
new { entity.GameId, entity.GameCollectionId });

            return insertedGameCollectionItemId;
        }
    }

    public override async Task<IEnumerable<GamesCollectionItem>> GetAllAsync()
    {
        using (var connection = CreateConnection())
        {
            var gamesCollectionsItems = await connection.QueryAsync<GamesCollectionItem, Game, GamesCollection, GamesCollectionItem>(@"
SELECT gci.Id, gci.GameId, gci.GameCollectionId,
g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer, g.LocalizationId,
gc.Id, gc.Name
FROM GamesCollectionsItems gci
LEFT JOIN Games g
on g.Id=gci.GameId
LEFT JOIN GamesCollections gc 
ON gc.Id=gci.GameCollectionId;", (gameCollectionItem, game, gameCollection) =>
            {
                if (game is not null && gameCollection is not null && gameCollectionItem is not null && !gameCollection.GamesCollectionItems.Any(g => g.GameId == game.Id))
                {
                    gameCollectionItem.Game = game;
                    gameCollectionItem.GameId = game.Id;
                    gameCollectionItem.GamesCollection = gameCollection;
                    gameCollectionItem.GamesCollectionId = gameCollection.Id;
                    gameCollection.GamesCollectionItems.Add(gameCollectionItem);
                }

                return gameCollectionItem;
            }, splitOn: "Id,Id");

            return gamesCollectionsItems;
        }
    }

    public override async Task<GamesCollectionItem> GetAsync(long id)
    {
        using (var connection = CreateConnection())
        {
            var gamesCollectionsItems = await connection.QueryAsync<GamesCollectionItem, Game, GamesCollectionItem>(@"SELECT gci.Id, gci.GameId, gci.GameCollectionId,
g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer, g.LocalizationId
FROM GamesCollectionsItems gci
LEFT JOIN Games g
on g.Id=gci.GameId
WHERE gci.Id=@Id;", (gameCollectionItem, game) =>
            {
                gameCollectionItem.Game = game;

                return gameCollectionItem;
            }, new { Id = id });

            var gamesCollectionsItemsResult = gamesCollectionsItems
                .GroupBy(b => new { b.GameId, b.GamesCollectionId })
                .Select(g =>
                {
                    GamesCollectionItem gameCollection = gamesCollectionsItems.First();
                    return gameCollection;
                });

            return gamesCollectionsItemsResult.SingleOrDefault();
        }
    }

    public override async Task<IEnumerable<GamesCollectionItem>> GetAsync(long offset, long limit)
    {
        using (var connection = CreateConnection())
        {
            var gamesCollectionsItems = await connection.QueryAsync<GamesCollectionItem, Game, GamesCollectionItem>(@"SELECT gci.Id, gci.GameId, gci.GameCollectionId,
g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer, g.LocalizationId
FROM GamesCollectionsItems gci
ON gc.Id=gci.GameCollectionId
LEFT JOIN Games g
on g.Id=gci.GameId
ORDER BY gc.Id ASC
OFFSET @Offset LIMIT @Limit;", (gameCollectionItem, game) =>
            {
                gameCollectionItem.Game = game;

                return gameCollectionItem;
            }, new { Offset = offset, Limit = limit });

            var gamesCollectionsItemsResult = gamesCollectionsItems
                .GroupBy(b => new { b.GameId, b.GamesCollectionId })
                .Select(g =>
                {
                    return g.First();
                });

            return gamesCollectionsItemsResult;
        }
    }

    public override async Task RemoveAsync(long id)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(@"DELETE FROM GamesCollectionsItems
WHERE Id=@Id;", new { Id = id });
        }
    }

    public override async Task<GamesCollectionItem> UpdateAsync(UpdateGamesCollectionItemModel entity, long id)
    {
        throw new NotImplementedException();
    }
}
