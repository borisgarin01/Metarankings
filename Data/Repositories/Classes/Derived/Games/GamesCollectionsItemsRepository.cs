using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesCollectionsItemsRepository(string connectionString) : Repository<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel>(connectionString)
{
    public override async Task<long> AddAsync(AddGamesCollectionItemModel entity, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var insertedGameCollectionItemId = await connection.QuerySingleAsync<long>(new CommandDefinition(@"
INSERT INTO GamesCollectionsItems (GameId, GameCollectionId)
VALUES (@GameId, @GameCollectionId)
RETURNING Id;", new { entity.GameId, entity.GameCollectionId }, cancellationToken: cancellationToken));

            return insertedGameCollectionItemId;
        }
    }

    public override async Task<IEnumerable<GamesCollectionItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var gamesCollectionsItems = await connection.QueryAsync<GamesCollectionItem, Game, GamesCollection, GamesCollectionItem>(new CommandDefinition(@"
SELECT gci.Id, gci.GameId, gci.GameCollectionId,
g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer, g.LocalizationId,
gc.Id, gc.Name
FROM GamesCollectionsItems gci
LEFT JOIN Games g
on g.Id=gci.GameId
LEFT JOIN GamesCollections gc 
ON gc.Id=gci.GameCollectionId;", cancellationToken: cancellationToken), (gameCollectionItem, game, gameCollection) =>
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

    public override async Task<GamesCollectionItem> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var gamesCollectionsItems = await connection.QueryAsync<GamesCollectionItem, Game, GamesCollectionItem>(new CommandDefinition(@"SELECT gci.Id, gci.GameId, gci.GameCollectionId,
g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer, g.LocalizationId
FROM GamesCollectionsItems gci
LEFT JOIN Games g
on g.Id=gci.GameId
WHERE gci.Id=@Id;", new { Id = id }, cancellationToken: cancellationToken), (gameCollectionItem, game) =>
            {
                gameCollectionItem.Game = game;

                return gameCollectionItem;
            });

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

    public override async Task<IEnumerable<GamesCollectionItem>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var gamesCollectionsItems = await connection.QueryAsync<GamesCollectionItem, Game, GamesCollectionItem>(new CommandDefinition(@"SELECT gci.Id, gci.GameId, gci.GameCollectionId,
g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer, g.LocalizationId
FROM GamesCollectionsItems gci
ON gc.Id=gci.GameCollectionId
LEFT JOIN Games g
on g.Id=gci.GameId
ORDER BY gc.Id ASC
OFFSET @Offset LIMIT @Limit;", new { Offset = offset, Limit = limit }, cancellationToken: cancellationToken), (gameCollectionItem, game) =>
            {
                gameCollectionItem.Game = game;

                return gameCollectionItem;
            });

            var gamesCollectionsItemsResult = gamesCollectionsItems
                .GroupBy(b => new { b.GameId, b.GamesCollectionId })
                .Select(g =>
                {
                    return g.First();
                });

            return gamesCollectionsItemsResult;
        }
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM GamesCollectionsItems
WHERE Id=@Id;", new { Id = id }, cancellationToken: cancellationToken));
        }
    }

    public override async Task<GamesCollectionItem> UpdateAsync(UpdateGamesCollectionItemModel entity, long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
