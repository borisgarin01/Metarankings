using Data.Repositories.Interfaces;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.Developers;
using Npgsql;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class DevelopersRepository : Repository<Developer, AddDeveloperModel, UpdateDeveloperModel>, IDevelopersRepository
{
    public DevelopersRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddDeveloperModel developer, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var id = await connection.QueryFirstAsync<long>(new CommandDefinition(@"INSERT INTO Developers
(Name)
VALUES (@Name)
RETURNING Id;", new
 {
     developer.Name
 }, cancellationToken: cancellationToken));
            return id;
        }
    }

    public override async Task<IEnumerable<Developer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var developersDictionary = new Dictionary<long, Developer>();
            var gamesDictionary = new Dictionary<long, Game>();

            await connection.QueryAsync<Developer, Game, Platform, Developer>(new CommandDefinition(@"
            select 
                developers.id, developers.name, 
                games.Id, games.Name, games.Image, 
                games.releasedate, 
                games.description, games.trailer,
                platforms.id, platforms.name
            from developers
            left join gamesdevelopers
                on gamesdevelopers.developerid=developers.id
            left join games
                on games.id=gamesdevelopers.gameid
            left join gamesplatforms
                on gamesplatforms.gameid=games.id
            left join platforms 
                on platforms.id=gamesplatforms.platformid", cancellationToken: cancellationToken),
                (developer, game, platform) =>
                {
                    if (!developersDictionary.TryGetValue(developer.Id, out var developerEntry))
                    {
                        developerEntry = developer;
                        developerEntry = developerEntry with { Games = new List<Game>() };
                        developersDictionary.Add(developerEntry.Id, developerEntry);
                    }

                    if (game != null)
                    {
                        if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                        {
                            gameEntry = game;
                            gameEntry.Platforms = new List<Platform>();
                            gamesDictionary.Add(gameEntry.Id, gameEntry);
                            developerEntry.Games.Add(gameEntry);
                        }

                        if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        {
                            gameEntry.Platforms.Add(platform);
                        }
                    }

                    return developerEntry;
                },
                splitOn: "Id,Id"  // Explicitly specify split points
            );

            return developersDictionary.Values;
        }
    }

    public override async Task<Developer> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var developersDictionary = new Dictionary<long, Developer>();
            var gamesDictionary = new Dictionary<long, Game>();

            await connection.QueryAsync<Developer, Game, Platform, Developer>(new CommandDefinition(@"
            select 
                developers.id, developers.name, 
                games.Id, games.Name, games.Image, 
                games.releasedate, 
                games.description, games.trailer,
                COALESCE((SELECT AVG(Score)::float FROM GamesPlayersReviews WHERE GameId = games.Id), 0) AS UsersScore,
                COALESCE((SELECT COUNT(*) FROM GamesPlayersReviews WHERE GameId = games.Id), 0) AS UsersReviewsCount,
                COALESCE((SELECT AVG(Score)::float FROM GamesCriticsReviews WHERE GameId = games.Id), 0) AS CriticsScore,
                COALESCE((SELECT COUNT(*) FROM GamesCriticsReviews WHERE GameId = games.Id), 0) AS CriticsReviewsCount,
                platforms.id, platforms.name
            from developers
            left join gamesdevelopers
                on gamesdevelopers.developerid=developers.id
            left join games
                on games.id=gamesdevelopers.gameid
            left join gamesplatforms
                on gamesplatforms.gameid=games.id
            left join platforms 
                on platforms.id=gamesplatforms.platformid
            WHERE developers.id=@id", new { id }, cancellationToken: cancellationToken),
                (developer, game, platform) =>
                {
                    if (!developersDictionary.TryGetValue(developer.Id, out var developerEntry))
                    {
                        developerEntry = developer;
                        developerEntry = developerEntry with { Games = new List<Game>() };
                        developersDictionary.Add(developerEntry.Id, developerEntry);
                    }

                    if (game != null)
                    {
                        if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                        {
                            gameEntry = game;
                            gameEntry.Platforms = new List<Platform>();
                            gamesDictionary.Add(gameEntry.Id, gameEntry);
                            developerEntry.Games.Add(gameEntry);
                        }

                        if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        {
                            gameEntry.Platforms.Add(platform);
                        }
                    }

                    return developerEntry;
                },
                splitOn: "Id,Id"
            );

            return developersDictionary.Values.FirstOrDefault();
        }
    }

    public override async Task<IEnumerable<Developer>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var developersDictionary = new Dictionary<long, Developer>();
            var gamesDictionary = new Dictionary<long, Game>();

            await connection.QueryAsync<Developer, Game, Platform, Developer>(new CommandDefinition(@"
            SELECT 
                developers.id, developers.name, 
                games.Id, games.Name, games.Image, 
                games.releasedate, 
                games.description, games.trailer,
                platforms.id, platforms.name
            FROM (
                SELECT id, name 
                FROM developers
                ORDER BY id asc
                OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY
            ) AS developers
            LEFT JOIN gamesdevelopers ON gamesdevelopers.developerid = developers.id
            LEFT JOIN games ON games.id = gamesdevelopers.gameid
            LEFT JOIN gamesplatforms ON gamesplatforms.gameid = games.id
            LEFT JOIN platforms ON platforms.id = gamesplatforms.platformid", new { Offset = offset, Limit = limit }, cancellationToken: cancellationToken),
                (developer, game, platform) =>
                {
                    if (!developersDictionary.TryGetValue(developer.Id, out var developerEntry))
                    {
                        developerEntry = developer;
                        developerEntry = developerEntry with
                        {
                            Games = new List<Game>()
                        };
                        developersDictionary.Add(developerEntry.Id, developerEntry);
                    }

                    if (game != null)
                    {
                        if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                        {
                            gameEntry = game;
                            gameEntry.Platforms = new List<Platform>();
                            gamesDictionary.Add(gameEntry.Id, gameEntry);
                            developerEntry.Games.Add(gameEntry);
                        }

                        if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        {
                            gameEntry.Platforms.Add(platform);
                        }
                    }

                    return developerEntry;
                },
                splitOn: "Id,Id"
            );

            return developersDictionary.Values;
        }
    }

    public async Task<Developer> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var developersDictionary = new Dictionary<long, Developer>();
        var gamesDictionary = new Dictionary<long, Game>();

        await connection.QueryAsync<Developer, Game, Platform, Developer>(new CommandDefinition(@"
            select 
                developers.id, developers.name, 
                games.Id, games.Name, games.Image, 
                games.releasedate, 
                games.description, games.trailer,
                platforms.id, platforms.name
            from developers
            left join gamesdevelopers
                on gamesdevelopers.developerid=developers.id
            left join games
                on games.id=gamesdevelopers.gameid
            left join gamesplatforms
                on gamesplatforms.gameid=games.id
            left join platforms 
                on platforms.id=gamesplatforms.platformid
            WHERE developers.name=@Name", new { Name = name }, cancellationToken: cancellationToken),
            (developer, game, platform) =>
            {
                if (!developersDictionary.TryGetValue(developer.Id, out var developerEntry))
                {
                    developerEntry = developer;
                    developerEntry = developerEntry with { Games = new List<Game>() };
                    developersDictionary.Add(developerEntry.Id, developerEntry);
                }

                if (game != null)
                {
                    if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Platforms = new List<Platform>();
                        gamesDictionary.Add(gameEntry.Id, gameEntry);
                        developerEntry.Games.Add(gameEntry);
                    }

                    if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                    {
                        gameEntry.Platforms.Add(platform);
                    }
                }

                return developerEntry;
            },
            splitOn: "Id,Id"
        );

        return developersDictionary.Values.FirstOrDefault();
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM 
Developers WHERE Id=@id", new { id }, cancellationToken: cancellationToken));
    }

    public override async Task<Developer> UpdateAsync(UpdateDeveloperModel developer, long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var updatedDeveloper = await connection.QueryFirstOrDefaultAsync<Developer>(new CommandDefinition(@"UPDATE Developers SET Name=@Name
WHERE Id=@id
RETURNING Name, Id;", new
            {
                developer.Name,
                id
            }, cancellationToken: cancellationToken));

            return updatedDeveloper;
        }
    }
}
