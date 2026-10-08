using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Publishers;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class PublishersRepository : Repository<Publisher, AddPublisherModel, UpdatePublisherModel>
{
    public PublishersRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddPublisherModel publisher, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        long id = await connection.QueryFirstAsync<long>(new CommandDefinition(@"INSERT INTO Publishers
(Name)
VALUES (@Name)
RETURNING Id;", new
{
    publisher.Name
}, cancellationToken: cancellationToken));
        return id;
    }

    public override async Task<IEnumerable<Publisher>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        Dictionary<long, Publisher> publisherDictionary = new Dictionary<long, Publisher>();

        await connection.QueryAsync<Publisher, Game, Publisher>(
            new CommandDefinition(@"SELECT 
                p.Id, p.Name,
                g.Id, g.Name, g.Image, g.LocalizationId,
                g.ReleaseDate, g.Description, g.Trailer
              FROM Publishers p
              LEFT JOIN GamesPublishers gp on gp.PublisherId = p.Id
              LEFT JOIN Games g ON g.Id = gp.GameId", cancellationToken: cancellationToken),
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

    public override async Task<Publisher> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        Dictionary<long, Publisher> publisherDictionary = new Dictionary<long, Publisher>();

        await connection.QueryAsync<Publisher, Game, Platform, Publisher>(
            new CommandDefinition(@"SELECT 
                p.Id, p.Name,
                g.Id, g.Name, g.Image, g.LocalizationId,
                g.ReleaseDate, g.Description, g.Trailer,
                COALESCE((SELECT AVG(Score)::float FROM GamesPlayersReviews WHERE GameId = g.Id), 0) AS UsersScore,
                COALESCE((SELECT COUNT(*) FROM GamesPlayersReviews WHERE GameId = g.Id), 0) AS UsersReviewsCount,
                COALESCE((SELECT AVG(Score)::float FROM GamesCriticsReviews WHERE GameId = g.Id), 0) AS CriticsScore,
                COALESCE((SELECT COUNT(*) FROM GamesCriticsReviews WHERE GameId = g.Id), 0) AS CriticsReviewsCount,
                platf.Id, platf.Name
              FROM Publishers p
              LEFT JOIN GamesPublishers gp on gp.PublisherId = p.Id
              LEFT JOIN Games g ON g.Id = gp.GameId
              LEFT JOIN GamesPlatforms
                ON GamesPlatforms.Gameid=g.id
              LEFT JOIN Platforms platf
                on platf.Id=GamesPlatforms.PlatformId
              WHERE p.Id = @id", new { id }, cancellationToken: cancellationToken),
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
            splitOn: "Id,Id"  // Split point between Publisher and Game columns
        );

        return publisherDictionary.Values.SingleOrDefault();
    }

    public override async Task<IEnumerable<Publisher>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        Dictionary<long, Publisher> publisherDictionary = new Dictionary<long, Publisher>();

        await connection.QueryAsync<Publisher, Game, Publisher>(new CommandDefinition(@"
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
            LEFT JOIN Games g ON g.Id = gp.GameId", new { offset, limit }, cancellationToken: cancellationToken),
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
            splitOn: "Id"
        );

        return publisherDictionary.Values;
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM 
Publishers WHERE Id=@id", new { id }, cancellationToken: cancellationToken));
    }

    public override async Task<Publisher> UpdateAsync(UpdatePublisherModel publisher, long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        Publisher? updatedPublisher = await connection.QueryFirstOrDefaultAsync<Publisher>(new CommandDefinition(@"UPDATE Publishers set Name=@Name 
RETURNING Name, Id
WHERE Id=@Id;", new
        {
            publisher.Name,
            id
        }, cancellationToken: cancellationToken));

        return updatedPublisher;
    }
}
