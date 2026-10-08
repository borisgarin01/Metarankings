using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Platforms;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class PlatformsRepository : Repository<Platform, AddPlatformModel, UpdatePlatformModel>
{
    public PlatformsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddPlatformModel platform, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        long id = await connection.QueryFirstAsync<long>(new CommandDefinition(@"INSERT INTO Platforms
(Name)
VALUES (@Name)
RETURNING Id;", new
{
    platform.Name
}, cancellationToken: cancellationToken));
        return id;
    }

    public override async Task<IEnumerable<Platform>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        Dictionary<long, Platform> platformDictionary = new Dictionary<long, Platform>();
        Dictionary<long, Game> gameDictionary = new Dictionary<long, Game>();

        await connection.QueryAsync<Platform, Game, Platform, Platform>(new CommandDefinition(@"
            SELECT 
                p1.Id, p1.Name,
                g.Id, g.Name, g.Image, g.LocalizationId,
                g.ReleaseDate, g.Description, g.Trailer,
                p2.Id, p2.Name
            FROM Platforms p1
            LEFT JOIN GamesPlatforms gp ON gp.PlatformId = p1.Id
            LEFT JOIN Games g ON g.Id = gp.GameId
            LEFT JOIN GamesPlatforms gp2 ON gp2.GameId = g.Id
            LEFT JOIN Platforms p2 ON p2.Id = gp2.PlatformId", cancellationToken: cancellationToken),
            (platform, game, gamePlatform) =>
            {
                // Get or create the platform
                if (!platformDictionary.TryGetValue(platform.Id, out var platformEntry))
                {
                    platformEntry = platform;
                    platformEntry.Games = new List<Game>();
                    platformDictionary.Add(platformEntry.Id, platformEntry);
                }

                if (game != null)
                {
                    // Get or create the game
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Platforms = new List<Platform>();
                        gameDictionary.Add(gameEntry.Id, gameEntry);

                        // Add game to platform if not already present
                        if (!platformEntry.Games.Any(g => g.Id == game.Id))
                        {
                            platformEntry.Games.Add(gameEntry);
                        }
                    }

                    // Add platform to game if it exists and isn't already added
                    if (gamePlatform != null && !gameEntry.Platforms.Any(p => p.Id == gamePlatform.Id))
                    {
                        gameEntry.Platforms.Add(gamePlatform);
                    }
                }

                return platformEntry;
            },
            splitOn: "Id,Id"
        );

        return platformDictionary.Values;
    }

    public override async Task<Platform> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        Dictionary<long, Platform> platformDictionary = new Dictionary<long, Platform>();
        Dictionary<long, Game> gameDictionary = new Dictionary<long, Game>();

        await connection.QueryAsync<Platform, Game, Platform, Platform>(new CommandDefinition(@"
        SELECT 
            p1.Id, p1.Name,
            g.Id, g.Name, g.Image, g.LocalizationId,
            g.ReleaseDate, g.Description, g.Trailer,
            COALESCE((SELECT AVG(Score)::float FROM GamesPlayersReviews WHERE GameId = g.Id), 0) AS UsersScore,
            COALESCE((SELECT COUNT(*) FROM GamesPlayersReviews WHERE GameId = g.Id), 0) AS UsersReviewsCount,
            COALESCE((SELECT AVG(Score)::float FROM GamesCriticsReviews WHERE GameId = g.Id), 0) AS CriticsScore,
            COALESCE((SELECT COUNT(*) FROM GamesCriticsReviews WHERE GameId = g.Id), 0) AS CriticsReviewsCount,
            p2.Id, p2.Name
        FROM Platforms p1
        LEFT JOIN GamesPlatforms gp ON gp.PlatformId = p1.Id
        LEFT JOIN Games g ON g.Id = gp.GameId
        LEFT JOIN GamesPlatforms gp2 ON gp2.GameId = g.Id
        LEFT JOIN Platforms p2 ON p2.Id = gp2.PlatformId
        WHERE p1.Id = @id", new { id }, cancellationToken: cancellationToken),
            (platform, game, gamePlatform) =>
            {
                if (!platformDictionary.TryGetValue(platform.Id, out var platformEntry))
                {
                    platformEntry = platform;
                    platformEntry.Games = new List<Game>();
                    platformDictionary.Add(platformEntry.Id, platformEntry);
                }

                if (game != null)
                {
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Platforms = new List<Platform>();
                        gameDictionary.Add(gameEntry.Id, gameEntry);

                        if (!platformEntry.Games.Any(g => g.Id == game.Id))
                        {
                            platformEntry.Games.Add(gameEntry);
                        }
                    }

                    if (gamePlatform != null && !gameEntry.Platforms.Any(p => p.Id == gamePlatform.Id))
                    {
                        gameEntry.Platforms.Add(gamePlatform);
                    }
                }

                return platformEntry;
            },
            splitOn: "Id,Id,Id"
        );

        return platformDictionary.Values.FirstOrDefault();
    }

    public override async Task<IEnumerable<Platform>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        Dictionary<long, Platform> platformDictionary = new Dictionary<long, Platform>();
        Dictionary<long, Game> gameDictionary = new Dictionary<long, Game>();

        await connection.QueryAsync<Platform, Game, Platform, Platform>(new CommandDefinition(@"
            SELECT 
                p1.Id, p1.Name,
                g.Id, g.Name, g.Image, g.LocalizationId,
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
            LEFT JOIN Platforms p2 ON p2.Id = gp2.PlatformId", new { offset, limit }, cancellationToken: cancellationToken),
            (platform, game, gamePlatform) =>
            {
                // Get or create the platform
                if (!platformDictionary.TryGetValue(platform.Id, out var platformEntry))
                {
                    platformEntry = platform;
                    platformEntry.Games = new List<Game>();
                    platformDictionary.Add(platformEntry.Id, platformEntry);
                }

                if (game != null)
                {
                    // Get or create the game
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Platforms = new List<Platform>();
                        gameDictionary.Add(gameEntry.Id, gameEntry);

                        // Add game to platform if not already present
                        platformEntry.Games.Add(gameEntry);
                    }

                    // Add platform to game if it exists and isn't already added
                    if (gamePlatform != null && !gameEntry.Platforms.Any(p => p.Id == gamePlatform.Id))
                    {
                        gameEntry.Platforms.Add(gamePlatform);
                    }
                }

                return platformEntry;
            },
            splitOn: "Id,Id"
        );

        return platformDictionary.Values;
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM 
Platforms WHERE Id=@id", new { id }, cancellationToken: cancellationToken));
    }

    public override async Task<Platform> UpdateAsync(UpdatePlatformModel platform, long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        var updatedPlatform = await connection.QueryFirstOrDefaultAsync<Platform>(new CommandDefinition(@"UPDATE Platforms set Name=@Name 
WHERE Id=@Id
RETURNING Name, Href, Id;", new
        {
            platform.Name,
            id
        }, cancellationToken: cancellationToken));

        return updatedPlatform;
    }
}
