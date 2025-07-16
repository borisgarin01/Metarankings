using Data.Repositories.Interfaces;
using Domain.Games;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace Data.Repositories.Classes.Derived.Games;
public sealed class GamesRepository : Repository, IRepository<Game>
{
    public GamesRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Game entity)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                List<Developer> insertedDevelopers = new();
                List<Genre> insertedGenres = new();
                Publisher insertedPublisher = null;
                Localization insertedLocalization = null;

                foreach (var developer in entity.Developers)
                {
                    var developerToFind = await connection.QueryFirstOrDefaultAsync<Developer>(@"SELECT Id, Name
FROM Developers
WHERE Name=@Name;", new { developer.Name }, transaction: transaction);

                    if (developerToFind is null)
                    {
                        var insertedDeveloper = await connection.QueryFirstAsync<Developer>(@"INSERT INTO Developers 
(Name)
output inserted.id, inserted.name
VALUES (@Name);", new { developer.Name }, transaction: transaction);
                        insertedDevelopers.Add(insertedDeveloper);
                    }
                    else
                        insertedDevelopers.Add(developerToFind);
                }
                foreach (var genre in entity.Genres)
                {
                    var genreToFind = await connection.QueryFirstOrDefaultAsync<Genre>(@"SELECT Id, Name
FROM Genres
WHERE Name=@Name;", new { genre.Name }, transaction: transaction);

                    if (genreToFind is null)
                    {
                        var insertedGenre = await connection.QueryFirstAsync<Genre>(@"INSERT INTO Genres 
(Name)
output inserted.id, inserted.name
VALUES (@Name);", new { genre.Name }, transaction: transaction);
                        insertedGenres.Add(insertedGenre);
                    }
                    else
                    {
                        insertedGenres.Add(genreToFind);
                    }
                }

                var publisherToFind = await connection.QueryFirstOrDefaultAsync<Publisher>(@"SELECT Id, Name 
FROM Publishers 
WHERE Name=@Name", new { entity.Publisher.Name }, transaction: transaction);

                if (publisherToFind is null)
                {
                    insertedPublisher = await connection.QueryFirstAsync<Publisher>(@"INSERT INTO Publishers (Name) 
output inserted.id, inserted.name
VALUES (@Name);", new { entity.Publisher.Name }, transaction: transaction);
                }
                else
                    insertedPublisher = publisherToFind;

                var localizationToFind = await connection.QueryFirstOrDefaultAsync<Localization>(@"SELECT Id, Name 
FROM Localizations WHERE Name=@Name", new { entity.Localization.Name }, transaction: transaction);

                if (localizationToFind is null)
                {
                    insertedLocalization = await connection.QueryFirstOrDefaultAsync<Localization>(@"INSERT INTO Localizations (Name)
output inserted.id, inserted.name
VALUES (@Name);", new { entity.Localization.Name }, transaction: transaction);
                }
                else
                    insertedLocalization = localizationToFind;

                var insertedGame = await connection.QueryFirstAsync<Game>(@"INSERT INTO Games 
(Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer) 
output inserted.Id, inserted.Name, inserted.Image, inserted.LocalizationId, inserted.PublisherId, inserted.ReleaseDate, inserted.Description, inserted.Trailer
VALUES
(@Name, @Image, @LocalizationId, @PublisherId, @ReleaseDate, @Description, @Trailer);", new
                {
                    entity.Name,
                    entity.Image,
                    LocalizationId = insertedLocalization.Id,
                    PublisherId = insertedPublisher.Id,
                    ReleaseDate = entity.ReleaseDate.Value,
                    entity.Description,
                    entity.Trailer
                }, transaction: transaction);

                foreach (var gameGenre in insertedGenres)
                {
                    var gameGenreToFind = await connection.QueryFirstOrDefaultAsync(@"SELECT Id, GameId, GenreId FROM GamesGenres WHERE GenreId=@GenreId and GameId=@GameId",
                        new
                        {
                            GenreId = gameGenre.Id,
                            GameId = insertedGame.Id
                        }, transaction: transaction);

                    if (gameGenreToFind is null)
                    {
                        var insertedGameGenre = await connection.QueryFirstAsync<GameGenre>(@"INSERT INTO GamesGenres (GameId, GenreId) 
OUTPUT inserted.Id, inserted.GameId, inserted.GenreId
VALUES (@GameId, @GenreId);", new { GameId = insertedGame.Id, GenreId = gameGenre.Id }, transaction: transaction);
                    }
                }

                foreach (var platform in entity.Platforms)
                {
                    var existingPlatform = await connection.QueryFirstOrDefaultAsync<Platform>(@"SELECT Id, Name 
FROM Platforms
WHERE Name=@Name;", new { platform.Name }, transaction: transaction);

                    if (existingPlatform is null)
                    {
                        existingPlatform = await connection.QueryFirstAsync<Platform>(@"INSERT INTO Platforms (Name)
output inserted.Id, inserted.Name
VALUES (@Name);", new { platform.Name }, transaction: transaction);
                    }

                    var existingGamePlatform = await connection.QueryFirstOrDefaultAsync<GamePlatform>(@"SELECT GameId, PlatformId FROM GamesPlatforms WHERE GameId=@GameId AND PlatformId=@PlatformId", new { GameId = insertedGame.Id, PlatformId = platform.Id }, transaction: transaction);

                    if (existingGamePlatform is null)
                    {
                        existingGamePlatform = await connection.QueryFirstAsync<GamePlatform>(@"INSERT INTO GamesPlatforms 
(GameId, PlatformId)
output inserted.GameId, inserted.PlatformId
VALUES (@GameId, @PlatformId);", new { GameId = insertedGame.Id, PlatformId = existingPlatform.Id }, transaction: transaction);
                    }
                }

                foreach (var developer in entity.Developers)
                {
                    var existingDeveloper = await connection.QueryFirstOrDefaultAsync<Developer>(@"SELECT Id, Name 
FROM Developers 
WHERE Name=@Name", new { developer.Name }, transaction: transaction);

                    if (existingDeveloper is null)
                    {
                        existingDeveloper = await connection.QueryFirstAsync<Developer>(@"INSERT INTO Developers (Name)
output inserted.Id, inserted.Name
VALUES (@Name)", new { Name = developer.Name }, transaction: transaction);
                    }
                    var insertedGameDeveloper = await connection.QueryAsync(@"INSERT INTO GamesDevelopers(GameId, DeveloperId)
output inserted.Id, inserted.GameId, inserted.DeveloperId
VALUES(@GameId, @DeveloperId)", new { GameId = insertedGame.Id, DeveloperId = existingDeveloper.Id }, transaction: transaction);
                }

                await transaction.CommitAsync();

                return insertedGame.Id;
            }
        }
    }

    public async Task AddRangeAsync(IEnumerable<Game> games)
    {
        foreach (var entity in games)
            await AddAsync(entity);
    }

    public async Task<IEnumerable<Game>> GetAsync(int offset, int limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
plat.id, plat.name, 
gs.id, gs.gameid
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
WHERE g.Id IN(SELECT Id FROM games ORDER BY Id ASC OFFSET @offset ROWS
            FETCH NEXT @limit ROWS ONLY)";

            var gamesDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gamesDictionary.TryGetValue(game.Name, out var gameEntry))
                    {
                        gameEntry = game;
                        gamesDictionary.Add(gameEntry.Name, gameEntry);
                    }

                    var developersDictionary = new Dictionary<long, Developer>();
                    var genresDictionary = new Dictionary<long, Genre>();
                    var platformsDictionary = new Dictionary<long, Platform>();
                    var screenshotsDictionary = new Dictionary<long, GameScreenshot>();

                    if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
                        developersDictionary.Add(developer.Id, developer);

                    if (genre is not null && !genresDictionary.TryGetValue(genre.Id, out var gen))
                        genresDictionary.Add(genre.Id, genre);
                    if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var plat))
                        platformsDictionary.Add(platform.Id, platform);

                    if (screenshot is not null && !screenshotsDictionary.TryGetValue(screenshot.Id, out var screen))
                        screenshotsDictionary.Add(screenshot.Id, screenshot);

                    if (publisher is not null && gameEntry.Publisher is null)
                        gameEntry = gameEntry with
                        {
                            Publisher = publisher,
                            Localization = localization,
                            Developers = developersDictionary.Values.ToList(),
                            Genres = genresDictionary.Values.ToList(),
                            Platforms = platformsDictionary.Values.ToList(),
                            Screenshots = screenshotsDictionary.Values.ToList(),
                        };

                    return gameEntry;
                },
                new { offset, limit },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gamesDictionary.Values.ToList();

            return result;
        }
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
plat.id, plat.name, 
gs.id, gs.gameid
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

            var gamesDictionary = new Dictionary<long, Game>();
            var developersDictionary = new Dictionary<long, Developer>();
            var genresDictionary = new Dictionary<long, Genre>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var gamesScreenshotsDictionary = new Dictionary<long, GameScreenshot>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gamesDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
                        developersDictionary.Add(developer.Id, developer);

                    if (genre is not null && !genresDictionary.TryGetValue(genre.Id, out var gen))
                        genresDictionary.Add(genre.Id, genre);

                    if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var platf))
                        platformsDictionary.Add(platform.Id, platform);

                    if (screenshot is not null && !gamesScreenshotsDictionary.TryGetValue(screenshot.Id, out var screensh))
                        gamesScreenshotsDictionary.Add(screenshot.Id, screenshot);

                    gameEntry = gameEntry with
                    {
                        Developers = developersDictionary.Values.ToList(),
                        Genres = genresDictionary.Values.ToList(),
                        Localization = (localization is not null && gameEntry.Localization == null) ? localization : null,
                        Platforms = platformsDictionary.Values.ToList(),
                        Publisher = (publisher is not null && gameEntry.Publisher == null) ? publisher : null,
                        Screenshots = gamesScreenshotsDictionary.Values.ToList()
                    };

                    return gameEntry;
                },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gamesDictionary.Values.ToList();

            return result;
        }
    }

    public async Task<Game> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name,
l.id, l.name,
plat.id, plat.name,
gs.id, gs.gameid
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

            var gameDictionary = new Dictionary<long, Game>();
            var developersDictionary = new Dictionary<long, Developer>();
            var genresDictionary = new Dictionary<long, Genre>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var screenshotsDictionary = new Dictionary<long, GameScreenshot>();

            var games = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameDictionary.Add(game.Id, gameEntry);
                    }

                    if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
                        developersDictionary.Add(developer.Id, developer);

                    if (genre is not null && !genresDictionary.TryGetValue(genre.Id, out var gen))
                        genresDictionary.Add(genre.Id, genre);

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        platformsDictionary.Add(platform.Id, platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        screenshotsDictionary.Add(screenshot.Id, screenshot);

                    gameEntry = gameEntry with
                    {
                        Developers = developersDictionary.Values.ToList(),
                        Genres = genresDictionary.Values.ToList(),
                        Localization = (localization is not null && gameEntry.Localization == null) ? localization : null,
                        Platforms = platformsDictionary.Values.ToList(),
                        Publisher = (publisher is not null && gameEntry.Publisher == null) ? publisher : null,
                        Screenshots = screenshotsDictionary.Values.ToList(),
                    };

                    return gameEntry;
                },
                new { id }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values.FirstOrDefault();

            return result;
        }
    }

    public async Task<IEnumerable<Game>> GetByGenreIdAsync(long genreId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
plat.id, plat.name, 
gs.id, gs.gameid
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
WHERE gen.Id=@genreId";

            var gamesDictionary = new Dictionary<long, Game>();
            var developersDictionary = new Dictionary<long, Developer>();
            var genresDictionary = new Dictionary<long, Genre>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var gamesScreenshotsDictionary = new Dictionary<long, GameScreenshot>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gamesDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
                        developersDictionary.Add(developer.Id, developer);

                    if (genre is not null && !genresDictionary.TryGetValue(genre.Id, out var gen))
                        genresDictionary.Add(genre.Id, genre);

                    if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var platf))
                        platformsDictionary.Add(platform.Id, platform);

                    if (screenshot is not null && !gamesScreenshotsDictionary.TryGetValue(screenshot.Id, out var screensh))
                        gamesScreenshotsDictionary.Add(screenshot.Id, screenshot);

                    gameEntry = gameEntry with
                    {
                        Developers = developersDictionary.Values.ToList(),
                        Genres = genresDictionary.Values.ToList(),
                        Localization = (localization is not null && gameEntry.Localization == null) ? localization : null,
                        Platforms = platformsDictionary.Values.ToList(),
                        Publisher = (publisher is not null && gameEntry.Publisher == null) ? publisher : null,
                        Screenshots = gamesScreenshotsDictionary.Values.ToList(),
                    };

                    return gameEntry;
                },
                new { genreId },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gamesDictionary.Values.ToList();

            return result;
        }
    }

    public async Task<IEnumerable<Game>> GetByPlatformIdAsync(long platformId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
plat.id, plat.name, 
gs.id, gs.gameid
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
WHERE plat.Id=@platformId";

            var gamesDictionary = new Dictionary<long, Game>();
            var developersDictionary = new Dictionary<long, Developer>();
            var genresDictionary = new Dictionary<long, Genre>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var gamesScreenshotsDictionary = new Dictionary<long, GameScreenshot>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gamesDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
                        developersDictionary.Add(developer.Id, developer);

                    if (genre is not null && !genresDictionary.TryGetValue(genre.Id, out var gen))
                        genresDictionary.Add(genre.Id, genre);

                    if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var platf))
                        platformsDictionary.Add(platform.Id, platform);

                    if (screenshot is not null && !gamesScreenshotsDictionary.TryGetValue(screenshot.Id, out var screensh))
                        gamesScreenshotsDictionary.Add(screenshot.Id, screenshot);

                    gameEntry = gameEntry with
                    {
                        Developers = developersDictionary.Values.ToList(),
                        Genres = genresDictionary.Values.ToList(),
                        Localization = (localization is not null && gameEntry.Localization == null) ? localization : null,
                        Platforms = platformsDictionary.Values.ToList(),
                        Publisher = (publisher is not null && gameEntry.Publisher == null) ? publisher : null,
                        Screenshots = gamesScreenshotsDictionary.Values.ToList(),
                    };

                    return gameEntry;
                },
                new { platformId },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gamesDictionary.Values.ToList();

            return result;
        }
    }

    public async Task<IEnumerable<Game>> GetByDeveloperIdAsync(long developerId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
plat.id, plat.name, 
gs.id, gs.gameid
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
WHERE gd.developerid = @developerId";

            var gamesDictionary = new Dictionary<long, Game>();
            var developersDictionary = new Dictionary<long, Developer>();
            var genresDictionary = new Dictionary<long, Genre>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var gamesScreenshotsDictionary = new Dictionary<long, GameScreenshot>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gamesDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
                        developersDictionary.Add(developer.Id, developer);

                    if (genre is not null && !genresDictionary.TryGetValue(genre.Id, out var gen))
                        genresDictionary.Add(genre.Id, genre);

                    if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var platf))
                        platformsDictionary.Add(platform.Id, platform);

                    if (screenshot is not null && !gamesScreenshotsDictionary.TryGetValue(screenshot.Id, out var screensh))
                        gamesScreenshotsDictionary.Add(screenshot.Id, screenshot);

                    gameEntry = gameEntry with
                    {
                        Developers = developersDictionary.Values.ToList(),
                        Genres = genresDictionary.Values.ToList(),
                        Localization = (localization is not null && gameEntry.Localization == null) ? localization : null,
                        Platforms = platformsDictionary.Values.ToList(),
                        Publisher = (publisher is not null && gameEntry.Publisher == null) ? publisher : null,
                        Screenshots = gamesScreenshotsDictionary.Values.ToList(),
                    };

                    return gameEntry;
                },
                new { developerId },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gamesDictionary.Values.ToList();

            return result;
        }
    }


    public async Task<IEnumerable<Game>> GetByPublisherIdAsync(long publisherId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
plat.id, plat.name, 
gs.id, gs.gameid
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
WHERE p.Id = @publisherId";

            var gamesDictionary = new Dictionary<long, Game>();
            var developersDictionary = new Dictionary<long, Developer>();
            var genresDictionary = new Dictionary<long, Genre>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var gamesScreenshotsDictionary = new Dictionary<long, GameScreenshot>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gamesDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
                        developersDictionary.Add(developer.Id, developer);

                    if (genre is not null && !genresDictionary.TryGetValue(genre.Id, out var gen))
                        genresDictionary.Add(genre.Id, genre);

                    if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var platf))
                        platformsDictionary.Add(platform.Id, platform);

                    if (screenshot is not null && !gamesScreenshotsDictionary.TryGetValue(screenshot.Id, out var screensh))
                        gamesScreenshotsDictionary.Add(screenshot.Id, screenshot);

                    gameEntry = gameEntry with
                    {
                        Developers = developersDictionary.Values.ToList(),
                        Genres = genresDictionary.Values.ToList(),
                        Localization = (localization is not null && gameEntry.Localization == null) ? localization : null,
                        Platforms = platformsDictionary.Values.ToList(),
                        Publisher = (publisher is not null && gameEntry.Publisher == null) ? publisher : null,
                        Screenshots = gamesScreenshotsDictionary.Values.ToList(),
                    };

                    return gameEntry;
                },
                new { publisherId },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gamesDictionary.Values.ToList();

            return result;
        }
    }

    public async Task<IEnumerable<Game>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
plat.id, plat.name, 
gs.id, gs.gameid
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
WHERE g.Id in 
(
    SELECT Id 
    FROM Games
    ORDER BY Id ASC
    OFFSET @offset ROWS
    FETCH NEXT @limit ROWS ONLY
);";

            var gamesDictionary = new Dictionary<long, Game>();
            var developersDictionary = new Dictionary<long, Developer>();
            var genresDictionary = new Dictionary<long, Genre>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var gamesScreenshotsDictionary = new Dictionary<long, GameScreenshot>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gamesDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
                        developersDictionary.Add(developer.Id, developer);

                    if (genre is not null && !genresDictionary.TryGetValue(genre.Id, out var gen))
                        genresDictionary.Add(genre.Id, genre);

                    if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var platf))
                        platformsDictionary.Add(platform.Id, platform);

                    if (screenshot is not null && !gamesScreenshotsDictionary.TryGetValue(screenshot.Id, out var screensh))
                        gamesScreenshotsDictionary.Add(screenshot.Id, screenshot);

                    gameEntry = gameEntry with
                    {
                        Developers = developersDictionary.Values.ToList(),
                        Genres = genresDictionary.Values.ToList(),
                        Localization = (localization is not null && gameEntry.Localization == null) ? localization : null,
                        Platforms = platformsDictionary.Values.ToList(),
                        Publisher = (publisher is not null && gameEntry.Publisher == null) ? publisher : null,
                        Screenshots = gamesScreenshotsDictionary.Values.ToList(),
                    };

                    return gameEntry;
                },
                new { offset, limit },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gamesDictionary.Values.ToList();

            return result;
        }
    }

    public Task RemoveAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        throw new NotImplementedException();
    }

    public Task<Game> UpdateAsync(Game entity, long id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Game>> GetByReleaseYearAsync(int year)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var sql = @"WITH FilteredGames AS (
    SELECT DISTINCT g.id
    FROM games g
    WHERE extract(year from g.ReleaseDate) = @year
)
SELECT
    g.Id, g.name, g.image, g.releasedate, g.description,
    d.id, d.name,
    p.id, p.name,
    gen.id, gen.name,
    l.id, l.name,
    plat.id, plat.name,
    gs.id, gs.gameid
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

            var gamesDictionary = new Dictionary<long, Game>();
            var developersDictionary = new Dictionary<long, Developer>();
            var genresDictionary = new Dictionary<long, Genre>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var gamesScreenshotsDictionary = new Dictionary<long, GameScreenshot>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gamesDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
                        developersDictionary.Add(developer.Id, developer);

                    if (genre is not null && !genresDictionary.TryGetValue(genre.Id, out var gen))
                        genresDictionary.Add(genre.Id, genre);

                    if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var platf))
                        platformsDictionary.Add(platform.Id, platform);

                    if (screenshot is not null && !gamesScreenshotsDictionary.TryGetValue(screenshot.Id, out var screensh))
                        gamesScreenshotsDictionary.Add(screenshot.Id, screenshot);

                    gameEntry = gameEntry with
                    {
                        Developers = developersDictionary.Values.ToList(),
                        Genres = genresDictionary.Values.ToList(),
                        Localization = (localization is not null && gameEntry.Localization == null) ? localization : null,
                        Platforms = platformsDictionary.Values.ToList(),
                        Publisher = (publisher is not null && gameEntry.Publisher == null) ? publisher : null,
                        Screenshots = gamesScreenshotsDictionary.Values.ToList(),
                    };

                    return gameEntry;
                },
                new { year },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gamesDictionary.Values.ToList();

            return result;
        }
    }

    public async Task<IEnumerable<Game>> GetByParametersAsync(GamesGettingRequestModel gamesGettingRequestModel)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            string initialQuery = @"SELECT
    g.Id, g.name, g.image, g.releasedate, g.description,
    d.id, d.name,
    p.id, p.name,
    gen.id, gen.name,
    l.id, l.name,
    plat.id, plat.name,
    gs.id, gs.gameid
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

            var gamesDictionary = new Dictionary<long, Game>();
            var developersDictionary = new Dictionary<long, Developer>();
            var genresDictionary = new Dictionary<long, Genre>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var gamesScreenshotsDictionary = new Dictionary<long, GameScreenshot>();

            var queryResults = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                query,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gamesDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gamesDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
                        developersDictionary.Add(developer.Id, developer);

                    if (genre is not null && !genresDictionary.TryGetValue(genre.Id, out var gen))
                        genresDictionary.Add(genre.Id, genre);

                    if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var platf))
                        platformsDictionary.Add(platform.Id, platform);

                    if (screenshot is not null && !gamesScreenshotsDictionary.TryGetValue(screenshot.Id, out var screensh))
                        gamesScreenshotsDictionary.Add(screenshot.Id, screenshot);

                    gameEntry = gameEntry with
                    {
                        Developers = developersDictionary.Values.ToList(),
                        Genres = genresDictionary.Values.ToList(),
                        Localization = (localization is not null && gameEntry.Localization == null) ? localization : null,
                        Platforms = platformsDictionary.Values.ToList(),
                        Publisher = (publisher is not null && gameEntry.Publisher == null) ? publisher : null,
                        Screenshots = gamesScreenshotsDictionary.Values.ToList(),
                    };

                    return gameEntry;
                },
                new { parameters },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gamesDictionary.Values.ToList();

            return result;
        }
    }
}
