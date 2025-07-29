using Dapper;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class LocalizationsRepository : Repository, ILocalizationsRepository
{
    public LocalizationsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Localization localization)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Localizations
(Name)
output inserted.id
VALUES (@Name);"
 , new
 {
     localization.Name,
 });
            return id;
        }
    }

    public async Task AddRangeAsync(IEnumerable<Localization> localizations)
    {
        foreach (var localization in localizations)
            await AddAsync(localization);
    }

    public async Task<IEnumerable<Localization>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localizationsDictionary = new Dictionary<string, Localization>();
            var gamesDictionary = new Dictionary<long, Game>();

            await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(@"
            SELECT 
                Localizations.Id, Localizations.Name,
                Games.Id, Games.Name, Games.Image, Games.LocalizationId, Games.PublisherId,
                Games.ReleaseDate, Games.Description, Games.Trailer,
                Platforms.Id, Platforms.Name,
                Developers.Id, Developers.Name,
                Publishers.Id, Publishers.Name
            FROM Localizations
                LEFT JOIN Games ON Games.LocalizationId = Localizations.Id
                LEFT JOIN GamesPlatforms ON GamesPlatforms.GameId = Games.Id
                LEFT JOIN Platforms ON Platforms.Id = GamesPlatforms.PlatformId
                LEFT JOIN GamesDevelopers ON GamesDevelopers.GameId = Games.Id
                LEFT JOIN Developers ON Developers.Id = GamesDevelopers.DeveloperId
                LEFT JOIN Publishers ON Publishers.Id = Games.PublisherId",
                (localization, game, platform, developer, publisher) =>
                {
                    // Get or create the localization entry
                    if (!localizationsDictionary.TryGetValue(localization.Name, out var localizationEntry))
                    {
                        localizationEntry = localization;
                        localizationEntry.Games = new List<Game>();
                        localizationsDictionary.Add(localization.Name, localizationEntry);
                    }

                    if (game != null)
                    {
                        // Get or create the game entry
                        if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                        {
                            gameEntry = game;
                            gameEntry.Platforms = new List<Platform>();
                            gameEntry.Developers = new List<Developer>();
                            gamesDictionary.Add(game.Id, gameEntry);

                            // Add game to localization if not already present
                            if (!localizationEntry.Games.Any(g => g.Id == game.Id))
                            {
                                localizationEntry.Games.Add(gameEntry);
                            }
                        }

                        // Add platform if it exists and isn't already added
                        if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        {
                            gameEntry.Platforms.Add(platform);
                        }

                        // Add developer if it exists and isn't already added
                        if (developer != null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        {
                            gameEntry.Developers.Add(developer);
                        }

                        // Set publisher if it exists and isn't already set
                        if (publisher != null && gameEntry.Publisher == null)
                        {
                            gameEntry.Publisher = publisher;
                        }
                    }

                    return localizationEntry;
                },
                splitOn: "Id,Id,Id,Id"  // Split points for each entity type
            );

            return localizationsDictionary.Values;
        }
    }

    public async Task<Localization> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localizationDictionary = new Dictionary<long, Localization>();
            var gamesDictionary = new Dictionary<long, Game>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var developersDictionary = new Dictionary<long, Developer>();
            var publishersDictionary = new Dictionary<long, Publisher>();

            var localization = await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(@"
            SELECT 
                loc.Id, loc.Name,
                g.Id, g.Name, g.Image, g.LocalizationId, g.PublisherId,
                g.ReleaseDate, g.Description, g.Trailer,
                p.Id, p.Name,
                d.Id, d.Name,
                pub.Id, pub.Name
            FROM Localizations loc
            LEFT JOIN Games g ON g.LocalizationId = loc.Id
            LEFT JOIN GamesPlatforms gp ON gp.GameId = g.Id
            LEFT JOIN Platforms p ON p.Id = gp.PlatformId
            LEFT JOIN GamesDevelopers gd ON gd.GameId = g.Id
            LEFT JOIN Developers d ON d.Id = gd.DeveloperId
            LEFT JOIN Publishers pub ON pub.Id = g.PublisherId
            WHERE loc.Id = @id",
                (loc, game, platform, developer, publisher) =>
                {
                    // Get or create localization
                    if (!localizationDictionary.TryGetValue(loc.Id, out var locEntry))
                    {
                        locEntry = loc;
                        locEntry.Games = new List<Game>();
                        localizationDictionary.Add(loc.Id, locEntry);
                    }

                    if (game != null)
                    {
                        // Get or create game
                        if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                        {
                            gameEntry = game;
                            gameEntry.Platforms = new List<Platform>();
                            gameEntry.Developers = new List<Developer>();
                            gamesDictionary.Add(game.Id, gameEntry);
                            locEntry.Games.Add(gameEntry);
                        }

                        // Add platform if exists
                        if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        {
                            gameEntry.Platforms.Add(platform);
                        }

                        // Add developer if exists
                        if (developer != null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        {
                            gameEntry.Developers.Add(developer);
                        }

                        // Set publisher if exists and not set
                        if (publisher != null && gameEntry.Publisher == null)
                        {
                            gameEntry.Publisher = publisher;
                        }
                    }

                    return locEntry;
                },
                new { id },
                splitOn: "Id,Id,Id,Id"
            );

            return localizationDictionary.Values.FirstOrDefault();
        }
    }

    public async Task<Localization> GetByPlatformAsync(long id, long platformId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localizationDictionary = new Dictionary<long, Localization>();
            var gamesDictionary = new Dictionary<long, Game>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var developersDictionary = new Dictionary<long, Developer>();
            var publishersDictionary = new Dictionary<long, Publisher>();

            var result = await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(@"
            SELECT 
                loc.Id, loc.Name,
                g.Id, g.Name, g.Image, g.LocalizationId, g.PublisherId,
                g.ReleaseDate, g.Description, g.Trailer,
                p.Id, p.Name,
                d.Id, d.Name,
                pub.Id, pub.Name
            FROM Localizations loc
            LEFT JOIN Games g ON g.LocalizationId = loc.Id AND g.Id IN (
                SELECT GameId FROM GamesPlatforms WHERE PlatformId = @platformId
            )
            LEFT JOIN GamesPlatforms gp ON gp.GameId = g.Id
            LEFT JOIN Platforms p ON p.Id = gp.PlatformId
            LEFT JOIN GamesDevelopers gd ON gd.GameId = g.Id
            LEFT JOIN Developers d ON d.Id = gd.DeveloperId
            LEFT JOIN Publishers pub ON pub.Id = g.PublisherId
            WHERE loc.Id = @id",
                (loc, game, platform, developer, publisher) =>
                {
                    // Get or create localization
                    if (!localizationDictionary.TryGetValue(loc.Id, out var locEntry))
                    {
                        locEntry = loc;
                        locEntry.Games = new List<Game>();
                        localizationDictionary.Add(loc.Id, locEntry);
                    }

                    if (game != null)
                    {
                        // Get or create game
                        if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                        {
                            gameEntry = game;
                            gameEntry.Platforms = new List<Platform>();
                            gameEntry.Developers = new List<Developer>();
                            gamesDictionary.Add(game.Id, gameEntry);
                            locEntry.Games.Add(gameEntry);
                        }

                        // Add platform if exists and matches our filter
                        if (platform != null && platform.Id == platformId &&
                            !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        {
                            gameEntry.Platforms.Add(platform);
                        }

                        // Add developer if exists
                        if (developer != null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        {
                            gameEntry.Developers.Add(developer);
                        }

                        // Set publisher if exists and not set
                        if (publisher != null && gameEntry.Publisher == null)
                        {
                            gameEntry.Publisher = publisher;
                        }
                    }

                    return locEntry;
                },
                new { id, platformId },
                splitOn: "Id,Id,Id,Id"
            );

            return localizationDictionary.Values.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Localization>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localizationDictionary = new Dictionary<long, Localization>();
            var gamesDictionary = new Dictionary<long, Game>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var developersDictionary = new Dictionary<long, Developer>();
            var publishersDictionary = new Dictionary<long, Publisher>();

            await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(@"
            SELECT 
                loc.Id, loc.Name,
                g.Id, g.Name, g.Image, g.LocalizationId, g.PublisherId,
                g.ReleaseDate, g.Description, g.Trailer,
                p.Id, p.Name,
                d.Id, d.Name,
                pub.Id, pub.Name
            FROM (
                SELECT Id, Name 
                FROM Localizations 
                ORDER BY Id
                OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY
            ) loc
            LEFT JOIN Games g ON g.LocalizationId = loc.Id
            LEFT JOIN GamesPlatforms gp ON gp.GameId = g.Id
            LEFT JOIN Platforms p ON p.Id = gp.PlatformId
            LEFT JOIN GamesDevelopers gd ON gd.GameId = g.Id
            LEFT JOIN Developers d ON d.Id = gd.DeveloperId
            LEFT JOIN Publishers pub ON pub.Id = g.PublisherId",
                (loc, game, platform, developer, publisher) =>
                {
                    if (!localizationDictionary.TryGetValue(loc.Id, out var locEntry))
                    {
                        locEntry = loc;
                        locEntry.Games = new List<Game>();
                        localizationDictionary.Add(loc.Id, locEntry);
                    }

                    if (game != null)
                    {
                        if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                        {
                            gameEntry = game;
                            gameEntry.Platforms = new List<Platform>();
                            gameEntry.Developers = new List<Developer>();
                            gamesDictionary.Add(game.Id, gameEntry);
                            locEntry.Games.Add(gameEntry);
                        }

                        if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        {
                            gameEntry.Platforms.Add(platform);
                        }

                        if (developer != null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        {
                            gameEntry.Developers.Add(developer);
                        }

                        if (publisher != null && gameEntry.Publisher == null)
                        {
                            gameEntry.Publisher = publisher;
                        }
                    }

                    return locEntry;
                },
                new { offset, limit },
                splitOn: "Id,Id,Id,Id"
            );

            return localizationDictionary.Values;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM 
Localizations WHERE Id=@id", new { id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<Localization> UpdateAsync(Localization localization, long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var updatedLocalization = await connection.QueryFirstOrDefaultAsync(@"UPDATE Localizations set Name=@Name
output inserted.name, inserted.href, inserted.id
where Id=@Id", new
            {
                localization.Name,
                id
            });

            return updatedLocalization;
        }
    }
}
