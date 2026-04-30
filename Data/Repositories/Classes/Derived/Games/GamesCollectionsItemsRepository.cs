using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesCollectionsItemsRepository : Repository, IRepository<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel>
{
    public GamesCollectionsItemsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddGamesCollectionItemModel entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var insertedGameCollectionItemId = await connection.QuerySingleAsync<long>(@"
INSERT INTO GamesCollectionsItems (GameId, GameCollectionId)
VALUES (@GameId, @GameCollectionId)
RETURNING Id;",
new { entity.GameId, entity.GameCollectionId });

            return insertedGameCollectionItemId;
        }
    }

    public async Task AddRangeAsync(IEnumerable<AddGamesCollectionItemModel> entities)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity);
        }
    }

    public async Task<IEnumerable<GamesCollectionItem>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
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
                if (game is not null)
                {
                    if (gameCollection is not null)
                    {
                        if (!gameCollection.Games.Any(g => g.Id == game.Id))
                        {
                            gameCollectionItem.Game = game;
                            gameCollectionItem.GameId = game.Id;
                            gameCollectionItem.GamesCollection = gameCollection;
                            gameCollectionItem.GamesCollectionId = gameCollection.Id;
                            gameCollection.Games.Add(game);
                        }
                    }
                }

                return gameCollectionItem;
            }, splitOn: "Id,Id");

            return gamesCollectionsItems;
        }
    }

    public async Task<GamesCollectionItem> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
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

    public async Task<IEnumerable<GamesCollectionItem>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
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

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM GamesCollectionsItems
WHERE Id=@Id;", new { Id = id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<GamesCollectionItem> UpdateAsync(UpdateGamesCollectionItemModel entity, long id)
    {
        throw new NotImplementedException();
    }
}
