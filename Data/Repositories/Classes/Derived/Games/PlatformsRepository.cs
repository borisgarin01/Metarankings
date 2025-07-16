using Data.Repositories.Interfaces;
using Domain.Games;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class PlatformsRepository : Repository, IRepository<Platform>
{
    public PlatformsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Platform platform)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Platforms
(Name)
output inserted.id
VALUES (@Name);"
 , new
 {
     platform.Name
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
        using (var connection = new SqlConnection(ConnectionString))
        {
            var platforms = await connection.QueryAsync<Platform, Game, Platform>(@"select Platforms.Id, Platforms.Name,
	Games.Id, Games.Name, Games.Image, Games.LocalizationId, 
	Games.PublisherId, Games.ReleaseDate, Games.Description,
	Games.Trailer
FROM Platforms
LEFT JOIN GamesPlatforms
	ON GamesPlatforms.PlatformId=Platforms.Id
LEFT JOIN Games
	ON Games.Id=GamesPlatforms.GameId", (platform, game) =>
            {
                platform.Games.Add(game);
                return platform;
            });

            var platformsResults = platforms.GroupBy(p => p.Id).Select(g =>
            {
                var groupedPlatform = g.First();
                groupedPlatform.Games = g.Select(p => p.Games.Single()).ToList();
                return groupedPlatform;
            });

            return platformsResults;
        }
    }

    public async Task<Platform> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var platformsDictionary = new Dictionary<long, Platform>();

            var platforms = await connection.QueryAsync<Platform, Game, Platform>(@"select Platforms.Id, Platforms.Name,
	Games.Id, Games.Name, Games.Image, Games.LocalizationId, 
	Games.PublisherId, Games.ReleaseDate, Games.Description,
	Games.Trailer
FROM Platforms
LEFT JOIN GamesPlatforms
	ON GamesPlatforms.PlatformId=Platforms.Id
LEFT JOIN Games
	ON Games.Id=GamesPlatforms.GameId
WHERE Platforms.Id=@id", (platform, game) =>
            {
                platform.Games.Add(game);
                return platform;
            }, new { id });

            var platformsResults = platforms.GroupBy(p => p.Id).Select(g =>
            {
                var groupedPlatform = g.First();
                groupedPlatform.Games = g.Select(p => p.Games.Single()).ToList();
                return groupedPlatform;
            });

            return platformsResults.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Platform>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var platforms = await connection.QueryAsync<Platform>(@"SELECT Id, Name 
FROM 
Platforms 
OFFSET @offset
LIMIT @limit;", new { offset, limit });

            return platforms;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
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
        using (var connection = new SqlConnection(ConnectionString))
        {
            var updatedPlatform = await connection.QueryFirstOrDefaultAsync<Platform>(@"UPDATE Platforms set Name=@Name 
output inserted.name, inserted.href, inserted.id
where Id=@id", new
            {
                platform.Name,
                id
            });

            return updatedPlatform;
        }
    }
}
