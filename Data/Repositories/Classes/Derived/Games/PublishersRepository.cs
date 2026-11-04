using Domain.Games;
using Domain.RequestsModels.Games.Publishers;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class PublishersRepository : Repository<Publisher, AddPublisherModel, UpdatePublisherModel>, IByNameGettingRepository<Publisher>
{
    public PublishersRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddPublisherModel publisher)
    {
        using (var connection = CreateConnection())
        {
            var id = await connection.QueryFirstAsync<long>(@"
                INSERT INTO Publishers (Name)
                VALUES (@Name)
                RETURNING Id;",
                new { publisher.Name });
            return id;
        }
    }

    public override async Task<IEnumerable<Publisher>> GetAllAsync()
    {
        using (var connection = CreateConnection())
        {
            var publishersDictionary = new Dictionary<long, Publisher>();
            var gamesDictionary = new Dictionary<long, Game>();

            await connection.QueryAsync<Publisher, Game, Platform, Publisher>(@"
                SELECT 
                    publishers.id, publishers.name, 
                    games.Id, games.Name, games.Image, 
                    games.releasedate, 
                    games.description, games.trailer,
                    platforms.id, platforms.name
                FROM publishers
                LEFT JOIN gamespublishers ON gamespublishers.publisherid = publishers.id
                LEFT JOIN games ON games.id = gamespublishers.gameid
                LEFT JOIN gamesplatforms ON gamesplatforms.gameid = games.id
                LEFT JOIN platforms ON platforms.id = gamesplatforms.platformid",
                (publisher, game, platform) =>
                {
                    if (!publishersDictionary.TryGetValue(publisher.Id, out var publisherEntry))
                    {
                        publisherEntry = publisher;
                        publisherEntry = publisherEntry with { Games = new List<Game>() };
                        publishersDictionary.Add(publisherEntry.Id, publisherEntry);
                    }

                    if (game != null)
                    {
                        if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                        {
                            gameEntry = game;
                            gameEntry.Platforms = new List<Platform>();
                            gamesDictionary.Add(gameEntry.Id, gameEntry);
                            publisherEntry.Games.Add(gameEntry);
                        }

                        if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        {
                            gameEntry.Platforms.Add(platform);
                        }
                    }

                    return publisherEntry;
                },
                splitOn: "Id,Id"
            );

            return publishersDictionary.Values;
        }
    }

    public override async Task<Publisher> GetAsync(long id)
    {
        using (var connection = CreateConnection())
        {
            var publishersDictionary = new Dictionary<long, Publisher>();
            var gamesDictionary = new Dictionary<long, Game>();

            await connection.QueryAsync<Publisher, Game, Platform, Publisher>(@"
                SELECT 
                    publishers.id, publishers.name, 
                    games.Id, games.Name, games.Image, 
                    games.releasedate, 
                    games.description, games.trailer,
                    COALESCE((SELECT AVG(Score)::float FROM GamesPlayersReviews WHERE GameId = games.Id), 0) AS UsersScore,
                    COALESCE((SELECT COUNT(*) FROM GamesPlayersReviews WHERE GameId = games.Id), 0) AS UsersReviewsCount,
                    COALESCE((SELECT AVG(Score)::float FROM GamesCriticsReviews WHERE GameId = games.Id), 0) AS CriticsScore,
                    COALESCE((SELECT COUNT(*) FROM GamesCriticsReviews WHERE GameId = games.Id), 0) AS CriticsReviewsCount,
                    platforms.id, platforms.name
                FROM publishers
                LEFT JOIN gamespublishers ON gamespublishers.publisherid = publishers.id
                LEFT JOIN games ON games.id = gamespublishers.gameid
                LEFT JOIN gamesplatforms ON gamesplatforms.gameid = games.id
                LEFT JOIN platforms ON platforms.id = gamesplatforms.platformid
                WHERE publishers.id = @id",
                (publisher, game, platform) =>
                {
                    if (!publishersDictionary.TryGetValue(publisher.Id, out var publisherEntry))
                    {
                        publisherEntry = publisher;
                        publisherEntry = publisherEntry with { Games = new List<Game>() };
                        publishersDictionary.Add(publisherEntry.Id, publisherEntry);
                    }

                    if (game != null)
                    {
                        if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                        {
                            gameEntry = game;
                            gameEntry.Platforms = new List<Platform>();
                            gamesDictionary.Add(gameEntry.Id, gameEntry);
                            publisherEntry.Games.Add(gameEntry);
                        }

                        if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        {
                            gameEntry.Platforms.Add(platform);
                        }
                    }

                    return publisherEntry;
                },
                splitOn: "Id,Id",
                param: new { id }
            );

            return publishersDictionary.Values.FirstOrDefault();
        }
    }

    public override async Task<IEnumerable<Publisher>> GetAsync(long offset, long limit)
    {
        using (var connection = CreateConnection())
        {
            var publishersDictionary = new Dictionary<long, Publisher>();
            var gamesDictionary = new Dictionary<long, Game>();

            await connection.QueryAsync<Publisher, Game, Platform, Publisher>(@"
                SELECT 
                    publishers.id, publishers.name, 
                    games.Id, games.Name, games.Image, 
                    games.releasedate, 
                    games.description, games.trailer,
                    platforms.id, platforms.name
                FROM (
                    SELECT id, name 
                    FROM publishers
                    ORDER BY id asc
                    OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY
                ) AS publishers
                LEFT JOIN gamespublishers ON gamespublishers.publisherid = publishers.id
                LEFT JOIN games ON games.id = gamespublishers.gameid
                LEFT JOIN gamesplatforms ON gamesplatforms.gameid = games.id
                LEFT JOIN platforms ON platforms.id = gamesplatforms.platformid",
                (publisher, game, platform) =>
                {
                    if (!publishersDictionary.TryGetValue(publisher.Id, out var publisherEntry))
                    {
                        publisherEntry = publisher;
                        publisherEntry = publisherEntry with { Games = new List<Game>() };
                        publishersDictionary.Add(publisherEntry.Id, publisherEntry);
                    }

                    if (game != null)
                    {
                        if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                        {
                            gameEntry = game;
                            gameEntry.Platforms = new List<Platform>();
                            gamesDictionary.Add(gameEntry.Id, gameEntry);
                            publisherEntry.Games.Add(gameEntry);
                        }

                        if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        {
                            gameEntry.Platforms.Add(platform);
                        }
                    }

                    return publisherEntry;
                },
                new { Offset = offset, Limit = limit },
                splitOn: "Id,Id"
            );

            return publishersDictionary.Values;
        }
    }

    public async Task<Publisher> GetByNameAsync(string name)
    {
        using var connection = CreateConnection();
        var publishersDictionary = new Dictionary<long, Publisher>();
        var gamesDictionary = new Dictionary<long, Game>();

        await connection.QueryAsync<Publisher, Game, Platform, Publisher>(@"
            SELECT 
                publishers.id, publishers.name, 
                games.Id, games.Name, games.Image, 
                games.releasedate, 
                games.description, games.trailer,
                platforms.id, platforms.name
            FROM publishers
            LEFT JOIN gamespublishers ON gamespublishers.publisherid = publishers.id
            LEFT JOIN games ON games.id = gamespublishers.gameid
            LEFT JOIN gamesplatforms ON gamesplatforms.gameid = games.id
            LEFT JOIN platforms ON platforms.id = gamesplatforms.platformid
            WHERE LOWER(TRIM(publishers.name)) = LOWER(TRIM(@Name))",
            (publisher, game, platform) =>
            {
                if (!publishersDictionary.TryGetValue(publisher.Id, out var publisherEntry))
                {
                    publisherEntry = publisher;
                    publisherEntry = publisherEntry with { Games = new List<Game>() };
                    publishersDictionary.Add(publisherEntry.Id, publisherEntry);
                }

                if (game != null)
                {
                    if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Platforms = new List<Platform>();
                        gamesDictionary.Add(gameEntry.Id, gameEntry);
                        publisherEntry.Games.Add(gameEntry);
                    }

                    if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                    {
                        gameEntry.Platforms.Add(platform);
                    }
                }

                return publisherEntry;
            },
            splitOn: "Id,Id",
            param: new { Name = name }
        );

        return publishersDictionary.Values.FirstOrDefault();
    }

    public override async Task RemoveAsync(long id)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(@"DELETE FROM Publishers WHERE Id = @id", new { id });
    }

    public override async Task<Publisher> UpdateAsync(UpdatePublisherModel publisher, long id)
    {
        using (var connection = CreateConnection())
        {
            var updatedPublisher = await connection.QueryFirstOrDefaultAsync<Publisher>(@"
                UPDATE Publishers 
                SET Name = @Name 
                WHERE Id = @id 
                RETURNING Name, Id;",
                new { publisher.Name, id });

            return updatedPublisher;
        }
    }
}