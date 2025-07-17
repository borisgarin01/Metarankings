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
            var platforms = await connection.QueryAsync<Platform, Game, Platform, Platform>(@"
            SELECT 
                p1.Id, p1.Name,
                g.Id, g.Name, g.Image, g.LocalizationId, g.PublisherId,
                g.ReleaseDate, g.Description, g.Trailer,
                p2.Id, p2.Name
            FROM (
                SELECT Id, Name 
                FROM Platforms 
                ORDER BY Id
                OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY
            ) p1
            LEFT JOIN GamesPlatforms gp ON gp.PlatformId = p1.Id
            LEFT JOIN Games g ON g.Id = gp.GameId
            LEFT JOIN GamesPlatforms gp2 ON gp2.GameId = g.Id
            LEFT JOIN Platforms p2 ON p2.Id = gp2.PlatformId",
                 (platform, game, gamePlatform) =>
                 {
                     // Get or create the platform
                     if (game is not null)
                     {
                         if (gamePlatform is not null)
                             game.Platforms.Add(gamePlatform);
                         platform.Games.Add(game);
                     }
                     return platform;
                 });

            var platformsResult = platforms.GroupBy(d => d.Id).Select(g =>
            {
                Platform groupedPlatform = g.First();

                groupedPlatform = groupedPlatform with
                {
                    Games = g.SelectMany(d => d.Games)
                    .GroupBy(g => g.Id)
                    .Select(gameGroup =>
                    {
                        var game = gameGroup.First();
                        game = game with
                        {
                            Platforms = gameGroup.SelectMany(g => g.Platforms)
                        .DistinctBy(p => p.Id).ToList()
                        };
                        return game;
                    }).ToList()
                };

                return groupedPlatform;
            });

            return platformsResult;
        }
    }

    public async Task<Platform> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var platforms = await connection.QueryAsync<Platform, Game, Platform>(@"
select Platforms.Id, Platforms.Name,
	Games.Id, Games.Name, Games.Image, Games.ReleaseDate, Games.Description,
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

            var platformsResult = platforms
                .GroupBy(d => d.Id)
                .Select(g =>
                {
                    Platform groupedPlatform = g.First() with
                    {
                        Games = g.SelectMany(d => d.Games).ToList()
                    };

                    return groupedPlatform;
                });

            return platformsResult.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Platform>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var platforms = await connection.QueryAsync<Platform, Game, Platform, Platform>(@"
            SELECT 
                p1.Id, p1.Name,
                g.Id, g.Name, g.Image, g.LocalizationId, g.PublisherId,
                g.ReleaseDate, g.Description, g.Trailer,
                p2.Id, p2.Name
            FROM (
                SELECT Id, Name 
                FROM Platforms 
                ORDER BY Id
                OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY
            ) p1
            LEFT JOIN GamesPlatforms gp ON gp.PlatformId = p1.Id
            LEFT JOIN Games g ON g.Id = gp.GameId
            LEFT JOIN GamesPlatforms gp2 ON gp2.GameId = g.Id
            LEFT JOIN Platforms p2 ON p2.Id = gp2.PlatformId",
                 (platform, game, gamePlatform) =>
                 {
                     // Get or create the platform
                     if (game is not null)
                     {
                         if (gamePlatform is not null)
                             game.Platforms.Add(gamePlatform);
                         platform.Games.Add(game);
                     }
                     return platform;
                 }, new { offset, limit });

            var platformsResult = platforms
                .GroupBy(d => d.Id)
                .Select(g =>
            {
                Platform groupedPlatform = g.First() with
                {
                    Games = g.SelectMany(d => d.Games)
                    .GroupBy(g => g.Id)
                    .Select(gameGroup =>
                    {
                        var game = gameGroup.First();
                        game = game with
                        {
                            Platforms = gameGroup.SelectMany(g => g.Platforms)
                        .DistinctBy(p => p.Id).ToList()
                        };
                        return game;
                    }).ToList()
                };

                return groupedPlatform;
            });

            return platformsResult;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
            await connection.ExecuteAsync(@"DELETE FROM 
Platforms WHERE Id=@id", new { id });
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
