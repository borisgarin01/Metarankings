using Domain.Games;
using Data.Repositories.Interfaces;
namespace Data.Repositories.Classes.Derived.Games;
public sealed class DevelopersRepository : Repository, IRepository<Developer>
{
    public DevelopersRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Developer developer)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Developers
(Name)
OUTPUT inserted.Id
VALUES (@Name);"
 , new
 {
     developer.Name
 });
            return id;
        }
    }

    public async Task AddRangeAsync(IEnumerable<Developer> developers)
    {
        foreach (var developer in developers)
        {
            await AddAsync(developer);
        }
    }

    public async Task<IEnumerable<Developer>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var developersDictionary = new Dictionary<long, Developer>();
            var gamesDictionary = new Dictionary<long, Game>();

            await connection.QueryAsync<Developer, Game, Platform, Developer>(@"
            select 
                developers.id, developers.name, 
                games.Id, games.Name, games.Image, 
                games.publisherId, games.releasedate, 
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
                on platforms.id=gamesplatforms.platformid",
                (developer, game, platform) =>
                {
                    if (!developersDictionary.TryGetValue(developer.Id, out var developerEntry))
                    {
                        developerEntry = developer;
                        developerEntry.Games = new List<Game>();
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

    public async Task<Developer> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var developersDictionary = new Dictionary<long, Developer>();
            var gamesDictionary = new Dictionary<long, Game>();

            await connection.QueryAsync<Developer, Game, Platform, Developer>(@"
            select 
                developers.id, developers.name, 
                games.Id, games.Name, games.Image, 
                games.publisherId, games.releasedate, 
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
            WHERE developers.id=@id",
                (developer, game, platform) =>
                {
                    if (!developersDictionary.TryGetValue(developer.Id, out var developerEntry))
                    {
                        developerEntry = developer;
                        developerEntry.Games = new List<Game>();
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
                splitOn: "Id,Id", // Explicitly specify split points
                param: new { id }
            );

            return developersDictionary.Values.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Developer>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var developersDictionary = new Dictionary<long, Developer>();
            var gamesDictionary = new Dictionary<long, Game>();

            await connection.QueryAsync<Developer, Game, Platform, Developer>(@"
            SELECT 
                developers.id, developers.name, 
                games.Id, games.Name, games.Image, 
                games.publisherId, games.releasedate, 
                games.description, games.trailer,
                platforms.id, platforms.name
            FROM (
                SELECT id, name 
                FROM developers
                ORDER BY id
                OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY
            ) AS developers
            LEFT JOIN gamesdevelopers ON gamesdevelopers.developerid = developers.id
            LEFT JOIN games ON games.id = gamesdevelopers.gameid
            LEFT JOIN gamesplatforms ON gamesplatforms.gameid = games.id
            LEFT JOIN platforms ON platforms.id = gamesplatforms.platformid",
                (developer, game, platform) =>
                {
                    if (!developersDictionary.TryGetValue(developer.Id, out var developerEntry))
                    {
                        developerEntry = developer;
                        developerEntry.Games = new List<Game>();
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
                new { Offset = offset, Limit = limit },  // This is where parameters are passed
                splitOn: "Id,Id"
            );

            return developersDictionary.Values;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM 
Developers WHERE Id=@id", new { id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<Developer> UpdateAsync(Developer developer, long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var updatedDeveloper = await connection.QueryFirstOrDefaultAsync<Developer>(@"UPDATE Developers set Name=@Name
OUTPUT inserted.Name, inserted.Id
where Id=@id", new
            {
                developer.Name,
                id
            });

            return updatedDeveloper;
        }
    }
}
