using Dapper;
using Data.Repositories.Interfaces;
using Domain.Games;
using Npgsql;
using System.Text;

namespace Data.Repositories.Classes.Derived.Games;
public sealed class GamesRepository : Repository, IRepository<GameModel>
{
    public GamesRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(GameModel entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            List<Developer> insertedDevelopers = new();
            List<Genre> insertedGenres = new();
            Publisher insertedPublisher = null;
            Localization insertedLocalization = null;

            foreach (var developer in entity.Developers)
            {
                var developerToFind = await connection.QueryFirstOrDefaultAsync<Developer>(@"SELECT Id, Name, Url
FROM Developers
WHERE Name=@Name;", new { developer.Name });

                if (developerToFind is null)
                {
                    var insertedDeveloper = await connection.QueryFirstAsync<Developer>(@"INSERT INTO Developers 
(Name, Url)
VALUES (@Name, @Url) RETURNING Id, Name, Url;", new { developer.Name, developer.Url });
                    insertedDevelopers.Add(insertedDeveloper);
                }
                else
                    insertedDevelopers.Add(developerToFind);
            }
            foreach (var genre in entity.Genres)
            {
                var genreToFind = await connection.QueryFirstOrDefaultAsync<Genre>(@"SELECT Id, Name, Url
FROM Genres
WHERE Name=@Name;", new { genre.Name });

                if (genreToFind is null)
                {
                    var insertedGenre = await connection.QueryFirstAsync<Genre>(@"INSERT INTO Genres 
(Name, Url)
VALUES (@Name, @Url)
RETURNING Id, Name, Url;", new { genre.Name, genre.Url });
                    insertedGenres.Add(insertedGenre);
                }
                else
                {
                    insertedGenres.Add(genreToFind);
                }
            }

            var publisherToFind = await connection.QueryFirstOrDefaultAsync<Publisher>(@"SELECT Id, Name, Url 
FROM Publishers 
WHERE Name=@Name", new { entity.Publisher.Name });

            if (publisherToFind is null)
            {
                insertedPublisher = await connection.QueryFirstAsync<Publisher>(@"INSERT INTO Publishers (Name, Url) 
VALUES (@Name, @Url)
RETURNING Id, Name, Url;", new { entity.Publisher.Name, entity.Publisher.Url });
            }
            else
                insertedPublisher = publisherToFind;

            var localizationToFind = await connection.QueryFirstOrDefaultAsync<Localization>(@"SELECT Id, Name, Href 
FROM Localizations WHERE Name=@Name", new { entity.Localization.Name });

            if (localizationToFind is null)
            {
                insertedLocalization = await connection.QueryFirstOrDefaultAsync<Localization>(@"INSERT INTO Localizations (Name, Href) 
VALUES (@Name, @Href) 
RETURNING Id, Name, Href;", new { entity.Localization.Name, entity.Localization.Href });
            }
            else
                insertedLocalization = localizationToFind;

            var insertedGame = await connection.QueryFirstAsync<Game>(@"INSERT INTO Games 
(Href, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer) 
VALUES
(@Href, @Name, @Image, @LocalizationId, @PublisherId, @ReleaseDate, @Description, @Trailer)
RETURNING Id, Href, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer;", new
            {
                entity.Href,
                entity.Name,
                entity.Image,
                LocalizationId = insertedLocalization.Id,
                PublisherId = insertedPublisher.Id,
                ReleaseDate = entity.ReleaseDate.Value,
                entity.Description,
                entity.Trailer
            });

            foreach (var gameGenre in insertedGenres)
            {
                var gameGenreToFind = await connection.QueryFirstOrDefaultAsync(@"SELECT Id, GameId, GenreId FROM GamesGenres WHERE GenreId=@GenreId and GameId=@GameId",
                    new
                    {
                        GenreId = gameGenre.Id,
                        GameId = entity.Id
                    });

                if (gameGenreToFind is null)
                {
                    var insertedGameGenre = await connection.QueryFirstAsync<GameGenre>(@"INSERT INTO GamesGenres (GameId, GenreId) 
VALUES (@GameId, @GenreId) 
RETURNING Id, GameId, GenreId;", new { GameId = insertedGame.Id, GenreId = gameGenre.Id });
                }
            }

            foreach (var platform in entity.Platforms)
            {
                var existingPlatform = await connection.QueryFirstOrDefaultAsync<Platform>(@"SELECT Id, Name, Href 
FROM Platforms
WHERE Name=@Name;", new { platform.Name });

                if (existingPlatform is null)
                {
                    existingPlatform = await connection.QueryFirstAsync<Platform>(@"INSERT INTO Platforms (Name, Href) VALUES (@Name, @Href)
RETURNING Id, Name, Href;", new { platform.Name, platform.Href });
                }

                var existingGamePlatform = await connection.QueryFirstOrDefaultAsync<GamePlatform>(@"SELECT GameId, PlatformId FROM GamesPlatforms WHERE GameId=@GameId AND PlatformId=@PlatformId", new { GameId = insertedGame.Id, PlatformId = platform.Id });

                if (existingGamePlatform is null)
                {
                    existingGamePlatform = await connection.QueryFirstAsync<GamePlatform>(@"INSERT INTO GamesPlatforms 
(GameId, PlatformId)
VALUES (@GameId, @PlatformId)
RETURNING GameId, PlatformId;", new { GameId = insertedGame.Id, PlatformId = existingPlatform.Id });
                }
            }

            foreach (var developer in entity.Developers)
            {
                var existingDeveloper = await connection.QueryFirstOrDefaultAsync<Developer>(@"SELECT Id, Name, Url FROM Developers WHERE Name=@Name", new { developer.Name });

                if (existingDeveloper is null)
                {
                    existingDeveloper = await connection.QueryFirstAsync<Developer>(@"INSERT INTO Developers (Name, Url) 
VALUES (@Name, @Url) 
RETURNING Id, Name, Url");
                }
                var insertedGameDeveloper = await connection.QueryAsync(@"INSERT INTO GamesDevelopers(GameId, DeveloperId) 
VALUES(@GameId, @DeveloperId) 
RETURNING Id, GameId, DeveloperId", new { GameId = insertedGame.Id, DeveloperId = existingDeveloper.Id });
            }

            return insertedGame.Id;
        }
    }

    public async Task AddRangeAsync(IEnumerable<GameModel> entities)
    {
        foreach (var entity in entities)
            await AddAsync(entity);
    }

    public async Task<IEnumerable<GameModel>> GetAsync(int offset, int limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
    g.Id, g.href, g.name, g.image, g.releasedate, g.description,
    d.id, d.name, d.url,
    p.id, p.name, p.url,
    gen.id, gen.name, gen.url,
    l.id, l.name, l.href,
    plat.id, plat.name, plat.href,
    gs.id, gs.url, gs.gameid
FROM (
    SELECT DISTINCT g.Id, g.href, g.name, g.image, g.releasedate, g.description, g.publisherid, g.localizationid
    FROM games g
    ORDER BY g.Id, g.href
    OFFSET @offset
    LIMIT @limit
) g
LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
LEFT JOIN developers d ON d.id = gd.developerid
LEFT JOIN publishers p ON p.id = g.publisherid
LEFT JOIN gamesgenres gg ON gg.gameid = g.id
LEFT JOIN genres gen ON gen.id = gg.genreid
LEFT JOIN localizations l ON l.id = g.localizationid
LEFT JOIN gamesplatforms gp ON gp.gameid = g.id
LEFT JOIN platforms plat ON plat.id = gp.platformid
LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id";

            var gameDictionary = new Dictionary<long, GameModel>();

            var query = await connection.QueryAsync<GameModel, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameModel>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && gameEntry.Publisher == null)
                        gameEntry.Publisher = publisher;

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    return gameEntry;
                },
                new { offset, limit },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values.ToList();

            return result;
        }
    }

    public async Task<IEnumerable<GameModel>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.href, g.name, g.image, g.releasedate, g.description,
d.id, d.name, d.url,
p.id, p.name, p.url,
gen.id, gen.name, gen.url,
l.id, l.name, l.href,
plat.id, plat.name, plat.href,
gs.id, gs.url, gs.gameid
    FROM games g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN developers d ON d.id = gd.developerid
    LEFT JOIN publishers p ON p.id = g.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gp ON gp.gameid = g.id
    LEFT JOIN platforms plat ON plat.id = gp.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id";

            var gameDictionary = new Dictionary<long, GameModel>();

            var query = await connection.QueryAsync<GameModel, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameModel>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && gameEntry.Publisher == null)
                        gameEntry.Publisher = publisher;

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    return gameEntry;
                },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values.ToList();

            return result;
        }
    }

    public async Task<GameModel> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.href, g.name, g.image, g.releasedate, g.description,
d.id, d.name, d.url,
p.id, p.name, p.url,
gen.id, gen.name, gen.url,
l.id, l.name, l.href,
plat.id, plat.name, plat.href,
gs.id, gs.url, gs.gameid
    FROM games g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN developers d ON d.id = gd.developerid
    LEFT JOIN publishers p ON p.id = g.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gp ON gp.gameid = g.id
    LEFT JOIN platforms plat ON plat.id = gp.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
WHERE g.Id=@id";

            var gameDictionary = new Dictionary<long, GameModel>();

            var query = await connection.QueryAsync<GameModel, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameModel>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && gameEntry.Publisher == null)
                        gameEntry.Publisher = publisher;

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    return gameEntry;
                },
                new { id }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values.FirstOrDefault();

            return result;
        }
    }

    public async Task<IEnumerable<GameModel>> GetByGenreUrlAsync(string genreUrl)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"WITH FilteredGames AS (
    SELECT DISTINCT g.id
    FROM games g
    JOIN gamesgenres gg ON gg.gameid = g.id
    JOIN genres gen ON gen.id = gg.genreid
    WHERE gen.url = @genreUrl
)
SELECT
    g.Id, g.href, g.name, g.image, g.releasedate, g.description,
    d.id, d.name, d.url,
    p.id, p.name, p.url,
    gen.id, gen.name, gen.url,
    l.id, l.name, l.href,
    plat.id, plat.name, plat.href,
    gs.id, gs.url, gs.gameid
FROM games g
LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
LEFT JOIN developers d ON d.id = gd.developerid
LEFT JOIN publishers p ON p.id = g.publisherid
LEFT JOIN gamesgenres gg ON gg.gameid = g.id
LEFT JOIN genres gen ON gen.id = gg.genreid
LEFT JOIN localizations l ON l.id = g.localizationid
LEFT JOIN gamesplatforms gp ON gp.gameid = g.id
LEFT JOIN platforms plat ON plat.id = gp.platformid
LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
WHERE g.id IN (SELECT id FROM FilteredGames)
ORDER BY g.id, gen.id;";

            var gameDictionary = new Dictionary<long, GameModel>();

            var query = await connection.QueryAsync<GameModel, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameModel>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && gameEntry.Publisher == null)
                        gameEntry.Publisher = publisher;

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    return gameEntry;
                },
                new { genreUrl }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }

    public async Task<IEnumerable<GameModel>> GetByPlatformUrlAsync(string platformUrl)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"WITH FilteredGames AS (
    SELECT DISTINCT g.id
    FROM games g
    JOIN gamesplatforms gp ON gp.gameid = g.id
    JOIN platforms plat ON plat.id = gp.platformid
    WHERE plat.href = @platformUrl
)
SELECT
    g.Id, g.href, g.name, g.image, g.releasedate, g.description,
    d.id, d.name, d.url,
    p.id, p.name, p.url,
    gen.id, gen.name, gen.url,
    l.id, l.name, l.href,
    plat.id, plat.name, plat.href,
    gs.id, gs.url, gs.gameid
FROM games g
LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
LEFT JOIN developers d ON d.id = gd.developerid
LEFT JOIN publishers p ON p.id = g.publisherid
LEFT JOIN gamesgenres gg ON gg.gameid = g.id
LEFT JOIN genres gen ON gen.id = gg.genreid
LEFT JOIN localizations l ON l.id = g.localizationid
LEFT JOIN gamesplatforms gp ON gp.gameid = g.id
LEFT JOIN platforms plat ON plat.id = gp.platformid
LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
WHERE g.id IN (SELECT id FROM FilteredGames)
ORDER BY g.id, gen.id;";

            var gameDictionary = new Dictionary<long, GameModel>();

            var query = await connection.QueryAsync<GameModel, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameModel>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && gameEntry.Publisher == null)
                        gameEntry.Publisher = publisher;

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    return gameEntry;
                },
                new { platformUrl }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }

    public async Task<IEnumerable<GameModel>> GetByDeveloperUrlAsync(string developerUrl)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"WITH FilteredGames AS (
    SELECT DISTINCT g.id
    FROM games g
    JOIN gamesdevelopers gp ON gp.gameid = g.id
    JOIN developers dev ON dev.id = gp.developerid
    WHERE dev.url = @developerUrl
)
SELECT
    g.Id, g.href, g.name, g.image, g.releasedate, g.description,
    d.id, d.name, d.url,
    p.id, p.name, p.url,
    gen.id, gen.name, gen.url,
    l.id, l.name, l.href,
    plat.id, plat.name, plat.href,
    gs.id, gs.url, gs.gameid
FROM games g
LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
LEFT JOIN developers d ON d.id = gd.developerid
LEFT JOIN publishers p ON p.id = g.publisherid
LEFT JOIN gamesgenres gg ON gg.gameid = g.id
LEFT JOIN genres gen ON gen.id = gg.genreid
LEFT JOIN localizations l ON l.id = g.localizationid
LEFT JOIN gamesplatforms gp ON gp.gameid = g.id
LEFT JOIN platforms plat ON plat.id = gp.platformid
LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
WHERE g.id IN (SELECT id FROM FilteredGames)
ORDER BY g.id, gen.id;";

            var gameDictionary = new Dictionary<long, GameModel>();

            var query = await connection.QueryAsync<GameModel, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameModel>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && gameEntry.Publisher == null)
                        gameEntry.Publisher = publisher;

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    return gameEntry;
                },
                new { developerUrl }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }


    public async Task<IEnumerable<GameModel>> GetByPublisherUrlAsync(string publisherUrl)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"WITH FilteredGames AS (
    SELECT DISTINCT g.id
    FROM games g
    JOIN publishers pub ON pub.id = g.publisherId
    WHERE pub.url = @publisherUrl
)
SELECT
    g.Id, g.href, g.name, g.image, g.releasedate, g.description,
    d.id, d.name, d.url,
    p.id, p.name, p.url,
    gen.id, gen.name, gen.url,
    l.id, l.name, l.href,
    plat.id, plat.name, plat.href,
    gs.id, gs.url, gs.gameid
FROM games g
LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
LEFT JOIN developers d ON d.id = gd.developerid
LEFT JOIN publishers p ON p.id = g.publisherid
LEFT JOIN gamesgenres gg ON gg.gameid = g.id
LEFT JOIN genres gen ON gen.id = gg.genreid
LEFT JOIN localizations l ON l.id = g.localizationid
LEFT JOIN gamesplatforms gp ON gp.gameid = g.id
LEFT JOIN platforms plat ON plat.id = gp.platformid
LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
WHERE g.id IN (SELECT id FROM FilteredGames)
ORDER BY g.id, gen.id;";

            var gameDictionary = new Dictionary<long, GameModel>();

            var query = await connection.QueryAsync<GameModel, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameModel>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && gameEntry.Publisher == null)
                        gameEntry.Publisher = publisher;

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    return gameEntry;
                },
                new { publisherUrl }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }

    public Task<IEnumerable<GameModel>> GetAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        throw new NotImplementedException();
    }

    public Task<GameModel> UpdateAsync(GameModel entity, long id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<GameModel>> GetByReleaseYearAsync(int year)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"WITH FilteredGames AS (
    SELECT DISTINCT g.id
    FROM games g
    WHERE extract(year from g.ReleaseDate) = @year
)
SELECT
    g.Id, g.href, g.name, g.image, g.releasedate, g.description,
    d.id, d.name, d.url,
    p.id, p.name, p.url,
    gen.id, gen.name, gen.url,
    l.id, l.name, l.href,
    plat.id, plat.name, plat.href,
    gs.id, gs.url, gs.gameid
FROM games g
LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
LEFT JOIN developers d ON d.id = gd.developerid
LEFT JOIN publishers p ON p.id = g.publisherid
LEFT JOIN gamesgenres gg ON gg.gameid = g.id
LEFT JOIN genres gen ON gen.id = gg.genreid
LEFT JOIN localizations l ON l.id = g.localizationid
LEFT JOIN gamesplatforms gp ON gp.gameid = g.id
LEFT JOIN platforms plat ON plat.id = gp.platformid
LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
WHERE g.id IN (SELECT id FROM FilteredGames)
ORDER BY g.id, gen.id;";

            var gameDictionary = new Dictionary<long, GameModel>();

            var query = await connection.QueryAsync<GameModel, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameModel>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && gameEntry.Publisher == null)
                        gameEntry.Publisher = publisher;

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    return gameEntry;
                },
                new { year }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }

    public async Task<IEnumerable<GameModel>> GetByParametersAsync(GamesGettingRequestModel gamesGettingRequestModel)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            string initialQuery = @"SELECT
    g.Id, g.href, g.name, g.image, g.releasedate, g.description,
    d.id, d.name, d.url,
    p.id, p.name, p.url,
    gen.id, gen.name, gen.url,
    l.id, l.name, l.href,
    plat.id, plat.name, plat.href,
    gs.id, gs.url, gs.gameid
FROM games g
LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
LEFT JOIN developers d ON d.id = gd.developerid
LEFT JOIN publishers p ON p.id = g.publisherid
LEFT JOIN gamesgenres gg ON gg.gameid = g.id
LEFT JOIN genres gen ON gen.id = gg.genreid
LEFT JOIN localizations l ON l.id = g.localizationid
LEFT JOIN gamesplatforms gp ON gp.gameid = g.id
LEFT JOIN platforms plat ON plat.id = gp.platformid
LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
WHERE 
g.Id in 
(SELECT games.id 
FROM games 
LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
LEFT JOIN developers d ON d.id = gd.developerid
LEFT JOIN publishers p ON p.id = g.publisherid
LEFT JOIN gamesgenres gg ON gg.gameid = g.id
LEFT JOIN genres gen ON gen.id = gg.genreid
LEFT JOIN localizations l ON l.id = g.localizationid
LEFT JOIN gamesplatforms gp ON gp.gameid = g.id
LEFT JOIN platforms plat ON plat.id = gp.platformid
LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
WHERE 1=1
";

            var queryStringBuilder = new StringBuilder(initialQuery);

            // Параметры для запроса
            var parameters = new DynamicParameters();

            // Добавляем условия в зависимости от заполненности фильтров
            if (gamesGettingRequestModel.ReleasesYears?.Any() == true)
            {
                queryStringBuilder.Append(" AND extract(year from g.ReleaseDate) = ANY(ARRAY[@ReleasesYears])");
                parameters.Add("ReleasesYears", gamesGettingRequestModel.ReleasesYears);
            }

            if (gamesGettingRequestModel.DevelopersIds?.Any() == true)
            {
                queryStringBuilder.Append(" AND gd.DeveloperId = ANY(ARRAY[@DevelopersIds])");
                parameters.Add("DevelopersIds", gamesGettingRequestModel.DevelopersIds);
            }

            if (gamesGettingRequestModel.GenresIds?.Any() == true)
            {
                queryStringBuilder.Append(" AND gg.GenreId = ANY(ARRAY[@GenresIds])");
                parameters.Add("GenresIds", gamesGettingRequestModel.GenresIds);
            }

            if (gamesGettingRequestModel.PublishersIds?.Any() == true)
            {
                queryStringBuilder.Append(" AND g.PublisherId = ANY(ARRAY[@PublishersIds])");
                parameters.Add("PublishersIds", gamesGettingRequestModel.PublishersIds);
            }

            if (gamesGettingRequestModel.PlatformsIds?.Any() == true)
            {
                queryStringBuilder.Append(" AND gp.PlatformId = ANY(ARRAY[@PlatformsIds])");
                parameters.Add("PlatformsIds", gamesGettingRequestModel.PlatformsIds);
            }

            queryStringBuilder.Append(");");

            var query = queryStringBuilder.ToString();

            var gameDictionary = new Dictionary<long, GameModel>();

            var games = await connection.QueryAsync<GameModel, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameModel>(
                queryStringBuilder.ToString(),
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer != null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher != null && gameEntry.Publisher == null)
                        gameEntry.Publisher = publisher;

                    if (genre != null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization != null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot != null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    return gameEntry;
                },
                parameters,
                splitOn: "Id,Id,Id,Id,Id,Id"
            );

            return gameDictionary.Values.ToArray();
        }
    }
}
