using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesCollectionsItemsRepository : Repository, IRepository<GameCollectionItem, AddGameCollectionItemModel, UpdateGameCollectionItemModel>
{
    public GamesCollectionsItemsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddGameCollectionItemModel entity)
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

    public async Task AddRangeAsync(IEnumerable<AddGameCollectionItemModel> entities)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity);
        }
    }

    public async Task<IEnumerable<GameCollectionItem>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var gamesCollectionsItems = await connection.QueryAsync<GameCollectionItem, Game, GameCollectionItem>(@"SELECT gci.Id, gci.Name,
g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer, g.LocalizationId
FROM GamesCollectionsItems gci
LEFT JOIN Games g
on g.Id=gci.GameId;", (gameCollectionItem, game) =>
            {
                gameCollectionItem.Game = game;

                return gameCollectionItem;
            });

            return gamesCollectionsItems;
        }
    }

    public async Task<GameCollectionItem> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var gamesCollectionsItems = await connection.QueryAsync<GameCollectionItem, Game, GameCollectionItem>(@"SELECT gci.Id, gci.Name,
g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer, g.LocalizationId
FROM GamesCollectionsItems gci
LEFT JOIN Games g
on g.Id=gci.GameId
WHERE gc.Id=@Id;", (gameCollectionItem, game) =>
            {
                gameCollectionItem.Game = game;

                return gameCollectionItem;
            }, new { Id = id });

            var gamesCollectionsItemsResult = gamesCollectionsItems
                .GroupBy(b => new { b.GameId, b.CollectionId })
                .Select(g =>
                {
                    GameCollectionItem gameCollection = gamesCollectionsItems.First();
                    return gameCollection;
                });

            return gamesCollectionsItemsResult.SingleOrDefault();
        }
    }

    public async Task<IEnumerable<GameCollectionItem>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var gamesCollectionsItems = await connection.QueryAsync<GameCollectionItem, Game, GameCollectionItem>(@"SELECT gci.Id, gci.Name,
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
                .GroupBy(b => new { b.GameId, b.CollectionId })
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

    public async Task<GameCollectionItem> UpdateAsync(UpdateGameCollectionItemModel entity, long id)
    {
        throw new NotImplementedException();
    }
}
