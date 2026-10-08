using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.RequestsModels.Games.Localizations;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class LocalizationsRepository : Repository<Localization, AddLocalizationModel, UpdateLocalizationModel>, ILocalizationsRepository
{
    public LocalizationsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddLocalizationModel localization, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        long id = await connection.QueryFirstAsync<long>(new CommandDefinition(@"INSERT INTO Localizations
(Name)
VALUES (@Name)
RETURNING Id;", new
{
    localization.Name,
}, cancellationToken: cancellationToken));
        return id;
    }

    public override async Task<IEnumerable<Localization>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        Dictionary<string, Localization> localizationsDictionary = new Dictionary<string, Localization>();
        Dictionary<long, Game> gamesDictionary = new Dictionary<long, Game>();

        await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(new CommandDefinition(@"
            SELECT 
                Localizations.Id, Localizations.Name,
                Games.Id, Games.Name, Games.Image, Games.LocalizationId,
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
                LEFT JOIN GamesPublishers gp ON gp.GameId = Games.Id
                LEFT JOIN Publishers on Publishers.Id = gp.PublisherId", cancellationToken: cancellationToken),
            (localization, game, platform, developer, publisher) =>
            {
                // Get or create the localization entry
                if (!localizationsDictionary.TryGetValue(localization.Name, out Localization? localizationEntry))
                {
                    localizationEntry = localization;
                    localizationEntry.Games = new List<Game>();
                    localizationsDictionary.Add(localization.Name, localizationEntry);
                }

                if (game != null)
                {
                    // Get or create the game entry
                    if (!gamesDictionary.TryGetValue(game.Id, out Game? gameEntry))
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
                    if (publisher != null && !gameEntry.Publishers.Any(d => d.Id == publisher.Id))
                    {
                        gameEntry.Publishers.Add(publisher);
                    }
                }

                return localizationEntry;
            },
            splitOn: "Id,Id,Id,Id"  // Split points for each entity type
        );

        return localizationsDictionary.Values;
    }

    public override async Task<Localization> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        Dictionary<long, Localization> localizationDictionary = new Dictionary<long, Localization>();
        Dictionary<long, Game> gamesDictionary = new Dictionary<long, Game>();
        Dictionary<long, Platform> platformsDictionary = new Dictionary<long, Platform>();
        Dictionary<long, Developer> developersDictionary = new Dictionary<long, Developer>();
        Dictionary<long, Publisher> publishersDictionary = new Dictionary<long, Publisher>();

        IEnumerable<Localization> localization = await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(new CommandDefinition(@"
            SELECT 
loc.Id, loc.Name,
g.Id, g.Name, g.Image, g.LocalizationId, g.ReleaseDate, g.Description, g.Trailer,                 
p.Id, p.Name,
d.Id, d.Name,
publ.Id, publ.Name
FROM Localizations loc
LEFT JOIN Games g ON g.LocalizationId = loc.Id
LEFT JOIN GamesPlatforms gplatf ON gplatf.GameId = g.Id
LEFT JOIN Platforms p ON p.Id = gplatf.PlatformId
LEFT JOIN GamesDevelopers gd ON gd.GameId = g.Id
LEFT JOIN Developers d ON d.Id = gd.DeveloperId
LEFT JOIN GamesPublishers gpubl ON gpubl.GameId = g.Id
LEFT JOIN Publishers publ on publ.Id = gpubl.PublisherId
WHERE loc.Id = @id", new { id }, cancellationToken: cancellationToken),
            (loc, game, platform, developer, publisher) =>
            {
                // Get or create localization
                if (!localizationDictionary.TryGetValue(loc.Id, out Localization? locEntry))
                {
                    locEntry = loc;
                    locEntry.Games = new List<Game>();
                    localizationDictionary.Add(loc.Id, locEntry);
                }

                if (game != null)
                {
                    // Get or create game
                    if (!gamesDictionary.TryGetValue(game.Id, out Game? gameEntry))
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
                    if (publisher != null && gameEntry.Publishers.Any(p => p.Id == publisher.Id))
                    {
                        gameEntry.Publishers.Add(publisher);
                    }
                }

                return locEntry;
            },
            splitOn: "Id,Id,Id,Id"
        );

        return localizationDictionary.Values.FirstOrDefault();
    }

    public async Task<Localization> GetByPlatformAsync(long id, long platformId, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        Dictionary<long, Localization> localizationDictionary = new Dictionary<long, Localization>();
        Dictionary<long, Game> gamesDictionary = new Dictionary<long, Game>();
        Dictionary<long, Platform> platformsDictionary = new Dictionary<long, Platform>();
        Dictionary<long, Developer> developersDictionary = new Dictionary<long, Developer>();
        Dictionary<long, Publisher> publishersDictionary = new Dictionary<long, Publisher>();

        IEnumerable<Localization> result = await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(new CommandDefinition(@"
            SELECT 
                loc.Id, loc.Name,
                g.Id, g.Name, g.Image, g.LocalizationId,
                g.ReleaseDate, g.Description, g.Trailer,
                p.Id, p.Name,
                d.Id, d.Name,
                publ.Id, publ.Name
            FROM Localizations loc
            LEFT JOIN Games g ON g.LocalizationId = loc.Id AND g.Id IN (
                SELECT GameId FROM GamesPlatforms WHERE PlatformId = @platformId
            )
            LEFT JOIN GamesPlatforms gp ON gp.GameId = g.Id
            LEFT JOIN Platforms p ON p.Id = gp.PlatformId
            LEFT JOIN GamesDevelopers gd ON gd.GameId = g.Id
            LEFT JOIN Developers d ON d.Id = gd.DeveloperId
            LEFT JOIN GamesPublishers gpubl ON gpubl.GameId = g.Id
            LEFT JOIN Publishers publ on publ.Id = gpubl.PublisherId
            WHERE loc.Id = @id", new { id, platformId }, cancellationToken: cancellationToken),
            (loc, game, platform, developer, publisher) =>
            {
                // Get or create localization
                if (!localizationDictionary.TryGetValue(loc.Id, out Localization? locEntry))
                {
                    locEntry = loc;
                    locEntry.Games = new List<Game>();
                    localizationDictionary.Add(loc.Id, locEntry);
                }

                if (game != null)
                {
                    // Get or create game
                    if (!gamesDictionary.TryGetValue(game.Id, out Game? gameEntry))
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
                    if (publisher != null && !gameEntry.Publishers.Any(d => d.Id == publisher.Id))
                    {
                        gameEntry.Publishers.Add(publisher);
                    }
                }

                return locEntry;
            },
            splitOn: "Id,Id,Id,Id"
        );

        return localizationDictionary.Values.FirstOrDefault();
    }

    public override async Task<IEnumerable<Localization>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        Dictionary<long, Localization> localizationDictionary = new Dictionary<long, Localization>();
        Dictionary<long, Game> gamesDictionary = new Dictionary<long, Game>();
        Dictionary<long, Platform> platformsDictionary = new Dictionary<long, Platform>();
        Dictionary<long, Developer> developersDictionary = new Dictionary<long, Developer>();
        Dictionary<long, Publisher> publishersDictionary = new Dictionary<long, Publisher>();

        await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(new CommandDefinition(@"
            SELECT 
                loc.Id, loc.Name,
                g.Id, g.Name, g.Image, g.LocalizationId,
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
            LEFT JOIN GamesPublishers gamesPub on gamesPub.GameId=g.Id
            LEFT JOIN Publishers pub ON pub.Id = gamesPub.PublisherId", new { offset, limit }, cancellationToken: cancellationToken),
            (loc, game, platform, developer, publisher) =>
            {
                if (!localizationDictionary.TryGetValue(loc.Id, out Localization? locEntry))
                {
                    locEntry = loc;
                    locEntry.Games = new List<Game>();
                    localizationDictionary.Add(loc.Id, locEntry);
                }

                if (game != null)
                {
                    if (!gamesDictionary.TryGetValue(game.Id, out Game? gameEntry))
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

                    if (publisher != null && !gameEntry.Publishers.Any(d => d.Id == publisher.Id))
                    {
                        gameEntry.Publishers.Add(publisher);
                    }
                }

                return locEntry;
            },
            splitOn: "Id,Id,Id,Id"
        );

        return localizationDictionary.Values;
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM 
Localizations WHERE Id=@id", new { id }, cancellationToken: cancellationToken));
    }

    public override async Task<Localization> UpdateAsync(UpdateLocalizationModel localization, long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        dynamic? updatedLocalization = await connection.QueryFirstOrDefaultAsync(new CommandDefinition(@"UPDATE Localizations set Name=@Name
WHERE Id=@Id
RETURNING Name, Href, Id;", new
        {
            localization.Name,
            id
        }, cancellationToken: cancellationToken));

        return updatedLocalization;
    }
}
