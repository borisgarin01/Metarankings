using Dapper;
using Data.Repositories.Interfaces;
using Domain;
using Npgsql;

namespace Data.Repositories.Classes.Derived;

public sealed class PlatformsRepository : Repository, IRepository<Platform>
{
    public PlatformsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Platform platform)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Platforms
(Name, Href)
VALUES (@Name, @Href)
RETURNING Id;"
 , new
 {
     platform.Name,
     platform.Href
 });
            return id;
        }
    }

    public async Task AddRangeAsync(IEnumerable<Platform> platfroms)
    {
        foreach (var platform in platfroms)
        {
            await AddAsync(platform);
        }
    }

    public async Task<IEnumerable<Platform>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var platforms = await connection.QueryAsync<Platform>(@"select 
platforms.id, platforms.name, platforms.href 
from platforms");

            if (platforms is null)
                return null;

            foreach (var platform in platforms)
            {
                var platformGames = await connection.QueryAsync<PlatformGame>(@"SELECT Id, GameId, PlatformId 
from GamesPlatforms 
where PlatformId=@platformId", new { platformId = platform.Id });

                foreach (var gamePlatform in platformGames)
                {
                    var games = await connection.QueryAsync<Game>(@"SELECT Id, Href, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer
FROM Games WHERE Id=@GameId", new { gamePlatform.GameId });

                    platform.Games = games;
                }

                foreach (var game in platform.Games)
                {
                    var gamePlatforms = await connection.QueryAsync<Platform>(@"SELECT platforms.Id, platforms.Name, platforms.Href 
FROM
platforms
INNER JOIN gamesPlatforms
on platforms.Id=gamesPlatforms.PlatformId
INNER JOIN games
on games.Id=gamesPlatforms.GameId
WHERE gamesPlatforms.GameId=@GameId", new { GameId = game.Id });

                    game.Platforms = platforms;
                }
            }
            return platforms;
        }
    }

    public async Task<Platform> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var platform = await connection.QueryFirstOrDefaultAsync<Platform>(@"select 
platforms.id, platforms.name, platforms.href 
from platforms
WHERE platforms.Id=@id", new { id });

            if (platform is null)
                return null;

            var platformGames = await connection.QueryAsync<PlatformGame>(@"SELECT Id, GameId, PlatformId 
from GamesPlatforms 
where PlatformId=@platformId", new { platformId = platform.Id });

            var games = new List<Game>();

            foreach (var gamePlatform in platformGames)
            {
                games.AddRange(await connection.QueryAsync<Game>(@"SELECT Id, Href, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer
FROM Games WHERE Id=@GameId", new { gamePlatform.GameId }));
            }

            platform.Games = games;

            return platform;
        }
    }

    public async Task<IEnumerable<Platform>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var platforms = await connection.QueryAsync<Platform>(@"SELECT Id, Name, Href 
FROM 
Platforms 
OFFSET @offset
LIMIT @limit;", new { offset, limit });

            return platforms;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM 
Platforms WHERE Id=@id", new { id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<Platform> UpdateAsync(Platform platform, long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var updatedPlatform = await connection.QueryFirstOrDefaultAsync<Platform>(@"UPDATE Platforms set Name=@Name, Href=@Href 
where Id=@id
returning Name, Href, Id", new
            {
                platform.Name,
                platform.Href,
                id
            });

            return updatedPlatform;
        }
    }
}
