using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;

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

        var gamesCollectionsDictionary = new Dictionary<long, GamesCollection>();

        await connection.QueryAsync<GamesCollection, Game, GamesCollection>(
            @"SELECT gc.Id, gc.Name, gc.Description,
                 g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer
          FROM GamesCollections gc
          LEFT JOIN GamesCollectionsItems gci ON gc.Id = gci.GameCollectionId
          LEFT JOIN Games g ON g.Id = gci.GameId
          ORDER BY gc.Id",
            (gameCollection, game) =>
            {
                if (!gamesCollectionsDictionary.TryGetValue(gameCollection.Id, out var existingCollection))
                {
                    // Initialize Games list
                    gameCollection.Games = new List<Game>();
                    gamesCollectionsDictionary.Add(gameCollection.Id, gameCollection);
                    existingCollection = gameCollection;
                }

                if (game != null)
                {
                    existingCollection.Games.Add(game);
                }

                return existingCollection;
            },
            splitOn: "Id");

        return gamesCollectionsDictionary.Values;
    }

    public async Task<GamesCollection?> GetAsync(long id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);

        var gamesCollection = await connection.QueryAsync<GamesCollection, Game, GamesCollection>(
            @"SELECT gc.Id, gc.Name, gc.Description,
                 g.Id, g.Name, g.Image, g.ReleaseDate, g.Description, g.Trailer
          FROM GamesCollections gc
          LEFT JOIN GamesCollectionsItems gci ON gc.Id = gci.GameCollectionId
          LEFT JOIN Games g ON g.Id = gci.GameId
          WHERE gc.Id = @Id",
            (gamesCollection, game) =>
            {
                if (!gamesCollection.Games.Any(b => b.Id == game.Id))
                    gamesCollection.Games.Add(game);
                return gamesCollection;
            },
            new { Id = id },
            splitOn: "Id");

        IEnumerable<GamesCollection> gamesCollectionGrouped = gamesCollection.GroupBy(b => new { b.Id })
                .Select(g =>
                {
                    GamesCollection gameCollection = g.First();
                    gameCollection.Games = g.SelectMany(b => b.Games).ToList();
                    return gameCollection;
                });

        return gamesCollectionGrouped.SingleOrDefault();
    }

    public async Task<IEnumerable<GamesCollection>> GetAsync(long offset, long limit)
    {
        using var connection = new NpgsqlConnection(ConnectionString);

        var gamesCollectionsDictionary = new Dictionary<long, GamesCollection>();

        await connection.QueryAsync<GamesCollection, Game, GamesCollection>(
            @"SELECT gc.Id, gc.Name, gc.Description,
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
            (gameCollection, game) =>
            {
                if (!gamesCollectionsDictionary.TryGetValue(gameCollection.Id, out var existingCollection))
                {
                    // Initialize Games list
                    gameCollection.Games = new List<Game>();
                    gamesCollectionsDictionary.Add(gameCollection.Id, gameCollection);
                    existingCollection = gameCollection;
                }

                if (game is not null)
                {
                    existingCollection.Games.Add(game);
                }

                return existingCollection;
            },
            new { offset, limit },
            splitOn: "Id");

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
