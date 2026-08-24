using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Publishers;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class PublishersRepository : Repository, IRepository<Publisher, AddPublisherModel, UpdatePublisherModel>
{
    public PublishersRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddPublisherModel publisher)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        long id = await connection.QueryFirstAsync<long>(@"INSERT INTO Publishers
(Name)
VALUES (@Name)
RETURNING Id;"
, new
{
    publisher.Name
});
        return id;
    }

    public async Task AddRangeAsync(IEnumerable<AddPublisherModel> publishers)
    {
        foreach (AddPublisherModel publisher in publishers)
        {
            await AddAsync(publisher);
        }
    }

    public async Task<IEnumerable<Publisher>> GetAllAsync()
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        Dictionary<long, Publisher> publisherDictionary = new Dictionary<long, Publisher>();

        await connection.QueryAsync<Publisher, Game, Publisher>(
            @"SELECT 
                p.Id, p.Name,
                g.Id, g.Name, g.Image, g.LocalizationId,
                g.ReleaseDate, g.Description, g.Trailer
              FROM Publishers p
              LEFT JOIN GamesPublishers gp on gp.PublisherId = p.Id
              LEFT JOIN Games g ON g.Id = gp.GameId",
            (publisher, game) =>
            {
                if (!publisherDictionary.TryGetValue(publisher.Id, out Publisher? publisherEntry))
                {
                    publisherEntry = publisher;
                    publisherEntry.Games = new List<Game>();
                    publisherDictionary.Add(publisherEntry.Id, publisherEntry);
                }

                if (game != null)
                {
                    publisherEntry.Games.Add(game);
                }

                return publisherEntry;
            },
            splitOn: "Id"  // Split point between Publisher and Game columns
        );

        return publisherDictionary.Values;
    }

    public async Task<Publisher> GetAsync(long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        Dictionary<long, Publisher> publisherDictionary = new Dictionary<long, Publisher>();

        await connection.QueryAsync<Publisher, Game, Platform, Publisher>(
            @"SELECT 
                p.Id, p.Name,
                g.Id, g.Name, g.Image, g.LocalizationId,
                g.ReleaseDate, g.Description, g.Trailer,
                platf.Id, platf.Name
              FROM Publishers p
              LEFT JOIN GamesPublishers gp on gp.PublisherId = p.Id
              LEFT JOIN Games g ON g.Id = gp.GameId
              LEFT JOIN GamesPlatforms
                ON GamesPlatforms.Gameid=g.id
              LEFT JOIN Platforms platf
                on platf.Id=GamesPlatforms.PlatformId
              WHERE p.Id = @id",
            (publisher, game, platform) =>
            {
                if (!publisherDictionary.TryGetValue(publisher.Id, out Publisher? publisherEntry))
                {
                    publisherEntry = publisher;
                    publisherEntry.Games = new List<Game>();
                    publisherDictionary.Add(publisherEntry.Id, publisherEntry);
                }

                if (game is not null && !publisherEntry.Games.Any(g => g.Id == game.Id))
                {
                    if (platform is not null)
                        game.Platforms.Add(platform);
                    publisherEntry.Games.Add(game);
                }

                return publisherEntry;
            },
            new { id },  // Correct parameter passing
            splitOn: "Id,Id"  // Split point between Publisher and Game columns
        );

        return publisherDictionary.Values.SingleOrDefault();
    }

    public async Task<IEnumerable<Publisher>> GetAsync(long offset, long limit)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        Dictionary<long, Publisher> publisherDictionary = new Dictionary<long, Publisher>();

        await connection.QueryAsync<Publisher, Game, Publisher>(@"
            SELECT 
                p.Id, p.Name,
                g.Id, g.Name, g.Image, g.LocalizationId,
                g.ReleaseDate, g.Description, g.Trailer
            FROM (
                SELECT Id, Name 
                FROM Publishers 
                ORDER BY Id
                OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY
            ) p
            LEFT JOIN GamesPublishers gp on gp.PublisherId = p.Id
            LEFT JOIN Games g ON g.Id = gp.GameId",
            (publisher, game) =>
            {
                if (!publisherDictionary.TryGetValue(publisher.Id, out Publisher? publisherEntry))
                {
                    publisherEntry = publisher;
                    publisherEntry.Games = new List<Game>();
                    publisherDictionary.Add(publisherEntry.Id, publisherEntry);
                }

                if (game != null)
                {
                    publisherEntry.Games.Add(game);
                }

                return publisherEntry;
            },
            new { offset, limit },
            splitOn: "Id"
        );

        return publisherDictionary.Values;
    }

    public async Task RemoveAsync(long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(@"DELETE FROM 
Publishers WHERE Id=@id", new { id });
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (long id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<Publisher> UpdateAsync(UpdatePublisherModel publisher, long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        Publisher? updatedPublisher = await connection.QueryFirstOrDefaultAsync<Publisher>(@"UPDATE Publishers set Name=@Name 
RETURNING Name, Id
WHERE Id=@Id;", new
        {
            publisher.Name,
            id
        });

        return updatedPublisher;
    }
}
