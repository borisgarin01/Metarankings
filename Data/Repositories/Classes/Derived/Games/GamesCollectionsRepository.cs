using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.Games.Collections;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesCollectionsRepository : Repository, IRepository<GameCollection, AddGameCollectionModel, UpdateGameCollectionModel>
{
    public GamesCollectionsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddGameCollectionModel entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var insertedGameCollectionId = await connection.QuerySingleAsync<long>(@"
INSERT INTO GamesCollections(Name) 
VALUES(@Name)
RETURNING Id;", new { entity.Name });

            return insertedGameCollectionId;
        }
    }

    public async Task AddRangeAsync(IEnumerable<AddGameCollectionModel> entities)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity);
        }
    }

    public async Task<IEnumerable<GameCollection>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var gamesCollectionsDictionary = new Dictionary<long, GameCollection>();

            await connection.QueryAsync<GameCollection, Game, GameCollection>(
                @"SELECT gc.Id, gc.Name,
                 g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer
          FROM GamesCollections gc
          LEFT JOIN GamesCollectionsItems gci ON gc.Id = gci.GameCollectionId
          LEFT JOIN Games g ON g.Id = gci.GameId
          ORDER BY gc.Id",
                (gameCollection, game) =>
                {
                    if (!gamesCollectionsDictionary.TryGetValue(gameCollection.Id, out GameCollection? existingCollection))
                    {
                        gamesCollectionsDictionary.Add(gameCollection.Id, gameCollection);
                    }

                    if (game is not null)
                        gameCollection.Games.Add(game);

                    return gameCollection;
                },
                splitOn: "Id");

            return gamesCollectionsDictionary.Values;
        }
    }

    public async Task<GameCollection?> GetAsync(long id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);

        var dictionary = new Dictionary<long, GameCollection>();

        await connection.QueryAsync<GameCollection, Game, GameCollection>(
            @"SELECT gc.Id, gc.Name,
                 g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer
          FROM GamesCollections gc
          LEFT JOIN GamesCollectionsItems gci ON gc.Id = gci.GameCollectionId
          LEFT JOIN Games g ON g.Id = gci.GameId
          WHERE gc.Id = @Id",
            (gc, g) =>
            {
                if (!dictionary.TryGetValue(gc.Id, out var collection))
                {
                    collection = gc;
                    collection.Games = new List<Game>();
                    dictionary.Add(gc.Id, collection);
                }

                if (g != null && !collection.Games.Any(b => b.Id == g.Id))
                {
                    collection.Games.Add(g);
                }

                return collection;
            },
            new { Id = id },
            splitOn: "Id");

        return dictionary.Values.SingleOrDefault();
    }

    public async Task<IEnumerable<GameCollection>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var gamesCollectionsDictionary = new Dictionary<long, GameCollection>();

            var gamesCollections = await connection.QueryAsync<GameCollection, Game, GameCollection>(
                @"SELECT gc.Id, gc.Name,
g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer
FROM GamesCollections gc
LEFT JOIN GamesCollectionsItems gci
ON gc.Id=gci.GameCollectionId
LEFT JOIN Games g
ON g.Id=gci.GameId
WHERE gc.Id in 
(SELECT Id 
FROM GamesCollections
OFFSET @offset LIMIT @limit
ORDER BY Id ASC);", (gameCollection, game) =>
                {
                    if (!gamesCollectionsDictionary.TryGetValue(gameCollection.Id, out var existingCollection))
                    {
                        gamesCollectionsDictionary.Add(gameCollection.Id, gameCollection);
                    }

                    if (game is not null)
                        gameCollection.Games.Add(game);

                    return gameCollection;
                }, new { offset, limit },
                splitOn: "Id");

            return gamesCollectionsDictionary.Values;
        }
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

    public async Task<GameCollection> UpdateAsync(UpdateGameCollectionModel entity, long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var updatedGameCollection = await connection.QuerySingleOrDefaultAsync<GameCollection>(@"UPDATE GamesCollections 
SET Name=@Name 
WHERE Id=@Id
RETURNING Name, Id;", new { Id = id });

            return updatedGameCollection;
        }
    }
}
