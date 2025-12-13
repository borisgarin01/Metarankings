using Dapper;
using Data.Extensions;
using Data.Repositories.Interfaces;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games;
using Domain.Reviews;
using IdentityLibrary.DTOs;

namespace Data.Repositories.Classes.Derived.Games;
public sealed class GamesRepository : Repository, IGamesRepository
{
    public GamesRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddGameModel entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                var insertedGame = await connection.QueryFirstAsync<Game>(@"INSERT INTO Games 
(Name, Image, LocalizationId, ReleaseDate, Description, Trailer) 
VALUES
(@Name, @Image, @LocalizationId, @ReleaseDate, @Description, @Trailer)
RETURNING Id, Name, Image, LocalizationId, ReleaseDate, Description, Trailer;", new
                {
                    entity.Name,
                    entity.Image,
                    entity.LocalizationId,
                    ReleaseDate = entity.ReleaseDate.Value,
                    entity.Description,
                    entity.Trailer
                }, transaction: transaction);

                foreach (var genreId in entity.GenresIds)
                {
                    var insertedGameGenre = await connection.QueryFirstAsync<GameGenre>(@"INSERT INTO GamesGenres (GameId, GenreId) 
VALUES (@GameId, @GenreId)
RETURNING Id, GameId, GenreId;", new { GameId = insertedGame.Id, GenreId = genreId }, transaction: transaction);
                }

                foreach (var publisherId in entity.PublishersIds)
                {
                    var insertedGamePublisher = await connection.QueryFirstAsync<GamePublisher>(@"INSERT INTO GamesPublishers (GameId, PublisherId) 
VALUES (@GameId, @PublisherId)
RETURNING GameId, PublisherId;", new { GameId = insertedGame.Id, PublisherId = publisherId }, transaction: transaction);
                }

                foreach (var platformId in entity.PlatformsIds)
                {
                    var insertedGamePlatform = await connection.QueryFirstAsync<GamePlatform>(@"INSERT INTO GamesPlatforms 
(GameId, PlatformId)
VALUES (@GameId, @PlatformId)
RETURNING GameId, PlatformId;", new { GameId = insertedGame.Id, PlatformId = platformId }, transaction: transaction);
                }

                foreach (var developerId in entity.DevelopersIds)
                {
                    var insertedGameDeveloper = await connection.QueryAsync(@"INSERT INTO GamesDevelopers(GameId, DeveloperId)
VALUES(@GameId, @DeveloperId)
RETURNING Id, GameId, DeveloperId;", new { GameId = insertedGame.Id, DeveloperId = developerId }, transaction: transaction);
                }

                await transaction.CommitAsync();

                return insertedGame.Id;
            }
        }
    }

    public async Task AddRangeAsync(IEnumerable<AddGameModel> games)
    {
        foreach (var entity in games)
            await AddAsync(entity);
    }

    public async Task<IEnumerable<Game>> GetFirstAsync(int offset, int limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
plat.id, plat.name, 
gs.Id, gs.GameId, gs.ImageUrl,
gc.Id, gc.Name, gc.Description
    FROM (select Id, Name, Image, ReleaseDate, Description, LocalizationId 
        from Games ORDER BY id asc
        OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY) as g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN developers d ON d.id = gd.developerid
    LEFT JOIN gamespublishers gpub on gpub.gameid=g.id
    LEFT JOIN publishers p on p.id = gpub.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
    LEFT JOIN platforms plat ON plat.id = gplatf.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
    LEFT JOIN gamescollectionsitems gci ON gci.GameId = g.Id
    LEFT JOIN gamescollections gc on gc.Id=gci.GameCollectionId";

            var gameDictionary = new Dictionary<long, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameCollection, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot, gameCollection) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out Game? gameEntry))
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

                    if (publisher is not null && !gameEntry.Publishers.Any(p => p.Id == publisher.Id))
                        gameEntry.Publishers.Add(publisher);

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    if (gameCollection is not null && !gameEntry.GameCollections.Any(b => b.Id == gameCollection.Id))
                        gameEntry.GameCollections.Add(gameCollection);

                    return gameEntry;
                }, new { offset, limit },
                splitOn: "Id,Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values.ToList();

            return result;
        }
    }
    public async Task<IEnumerable<Game>> GetLastAsync(int offset, int limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
plat.id, plat.name, 
gs.Id, gs.GameId, gs.ImageUrl,
gc.Id, gc.Name, gc.Description
    FROM (select Id, Name, Image, ReleaseDate, Description, LocalizationId 
        from Games ORDER BY id desc
        OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY) as g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN developers d ON d.id = gd.developerid
    LEFT JOIN gamespublishers gpub on gpub.gameid=g.id
    LEFT JOIN publishers p on p.id = gpub.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
    LEFT JOIN platforms plat ON plat.id = gplatf.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
    LEFT JOIN gamescollectionsitems gci ON gci.GameId = g.Id
    LEFT JOIN gamescollections gc on gc.Id=gci.GameCollectionId";

            var gameDictionary = new Dictionary<long, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameCollection, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot, gameCollection) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out Game? gameEntry))
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

                    if (publisher is not null && !gameEntry.Publishers.Any(p => p.Id == publisher.Id))
                        gameEntry.Publishers.Add(publisher);

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    if (gameCollection is not null && !gameEntry.GameCollections.Any(b => b.Id == gameCollection.Id))
                        gameEntry.GameCollections.Add(gameCollection);

                    return gameEntry;
                }, new { offset, limit },
                splitOn: "Id,Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values.ToList();

            return result;
        }
    }


    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
platf.id, platf.name, 
gs.id, gs.gameid, gs.imageUrl,
gc.Id, gc.Name, gc.Description
    FROM games g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN developers d ON d.id = gd.developerid
    LEFT JOIN gamespublishers gpub on gpub.gameid=g.id
    LEFT JOIN publishers p on p.id = gpub.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
    LEFT JOIN platforms platf ON platf.id = gplatf.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
    LEFT JOIN gamescollectionsitems gci ON gci.GameId = g.Id
    LEFT JOIN gamescollections gc on gc.Id=gci.GameCollectionId";

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameCollection, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot, gameCollection) =>
                {
                    if (!gameDictionary.TryGetValue(game.Name, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameEntry.Publishers = new List<Publisher>();
                        gameEntry.GameCollections = new List<GameCollection>();
                        gameDictionary.Add(gameEntry.Name, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && !gameEntry.Publishers.Any(p => p.Id == publisher.Id))
                        gameEntry.Publishers.Add(publisher);

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    if (gameCollection is not null && !gameEntry.GameCollections.Any(b => b.Id == gameCollection.Id))
                        gameEntry.GameCollections.Add(gameCollection);

                    return gameEntry;
                },
                splitOn: "Id,Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values.ToList();

            return result;
        }
    }

    public async Task<Game> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
platf.id, platf.name, 
gs.id, gs.gameid, gs.imageUrl,
gc.Id, gc.Name, gc.Description,
gpr.Id, gpr.GameId, gpr.UserId, gpr.Score, gpr.TextContent, gpr.Date,
au.Id, au.UserName, au.NormalizedUserName, au.Email, au.NormalizedEmail, au.EmailConfirmed, au.PasswordHash, au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
    FROM games g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN developers d ON d.id = gd.developerid
    LEFT JOIN gamespublishers gpub on gpub.gameid=g.id
    LEFT JOIN publishers p on p.id = gpub.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
    LEFT JOIN platforms platf ON platf.id = gplatf.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
    LEFT JOIN gamescollectionsitems gci ON gci.GameId = g.Id
    LEFT JOIN gamescollections gc on gc.Id=gci.GameCollectionId
    LEFT JOIN GamesPlayersReviews gpr on gpr.gameid=g.Id
    LEFT JOIN ApplicationUsers au on au.Id=gpr.UserId
WHERE g.Id=@id";

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameCollection, GameReview, ApplicationUser, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot, gameCollection, gamePlayerReview, applicationUser) =>
                {
                    if (!gameDictionary.TryGetValue(game.Name, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameEntry.GamesPlayersReviews = new List<GameReview>();
                        gameEntry.Publishers = new List<Publisher>();
                        gameDictionary.Add(gameEntry.Name, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && !gameEntry.Publishers.Any(p => p.Id == publisher.Id))
                        gameEntry.Publishers.Add(publisher);

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    if (gamePlayerReview is not null && applicationUser is not null)
                    {
                        gamePlayerReview = gamePlayerReview with { ApplicationUser = applicationUser };

                        if (!gameEntry.GamesPlayersReviews.Any(s => s.Id == gamePlayerReview.Id))
                            gameEntry.GamesPlayersReviews.Add(gamePlayerReview);
                    }

                    if (gameCollection is not null && !gameEntry.GameCollections.Any(b => b.Id == gameCollection.Id))
                        gameEntry.GameCollections.Add(gameCollection);

                    return gameEntry;
                },
                new { id } // Parameter passed here
            );

            var result = gameDictionary.Values.FirstOrDefault();

            return result;
        }
    }

    public async Task<IEnumerable<Game>> GetByGenreIdAsync(long genreId)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"WITH FilteredGames AS (
    SELECT DISTINCT g.id
    FROM games g
    JOIN gamesgenres gg ON gg.gameid = g.id
    gg.genreId=@genreId
)
SELECT
    g.Id, g.name, g.image, g.releasedate, g.description,
    d.id, d.name,
    p.id, p.name,
    gen.id, gen.name,
    l.id, l.name,
    plat.id, plat.name,
    gs.Id, gs.GameId, gs.ImageUrl,
    gc.Id, gc.Name, gc.Description
FROM games g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN developers d ON d.id = gd.developerid
    LEFT JOIN gamespublishers gpub on gpub.gameid=g.id
    LEFT JOIN publishers p on p.id = gpub.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
    LEFT JOIN platforms plat ON plat.id = gp.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
    LEFT JOIN gamescollections gc on gc.gameid = g.id
WHERE g.id IN (SELECT id FROM FilteredGames)
ORDER BY g.id, gen.id;";

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameCollection, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot, gameCollection) =>
                {
                    if (!gameDictionary.TryGetValue(game.Name, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameDictionary.Add(gameEntry.Name, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && !gameEntry.Publishers.Any(p => p.Id == publisher.Id))
                        gameEntry.Publishers.Add(publisher);

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    if (gameCollection is not null && !gameEntry.GameCollections.Any(gc => gc.Id == gameCollection.Id))
                        gameEntry.GameCollections.Add(gameCollection);

                    return gameEntry;
                },
                new { genreId }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }

    public async Task<IEnumerable<Game>> GetByPlatformIdAsync(long platformId)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"WITH FilteredGames AS (
    SELECT DISTINCT g.id
    FROM games g
    JOIN gamesplatforms gp ON gp.gameid = g.id
    WHERE gp.platformId = @platformId
)
SELECT
    g.Id, g.name, g.image, g.releasedate, g.description,
    d.id, d.name, 
    p.id, p.name, 
    gen.id, gen.name, 
    l.id, l.name,
    plat.id, plat.name,
    gs.Id, gs.GameId, gs.ImageUrl,
    gc.Id, gc.Name, gc.Description
FROM games g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN developers d ON d.id = gd.developerid
    LEFT JOIN gamespublishers gpub on gpub.gameid=g.id
    LEFT JOIN publishers p on p.id = gp.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
    LEFT JOIN platforms plat ON plat.id = gp.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
    LEFT JOIN gamescollections gc on gc.gameid = g.id
WHERE g.id IN (SELECT id FROM FilteredGames)
ORDER BY g.id, gen.id;";

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameCollection, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot, gameCollection) =>
                {
                    if (!gameDictionary.TryGetValue(game.Name, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameDictionary.Add(gameEntry.Name, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && !gameEntry.Publishers.Any(p => p.Id == publisher.Id))
                        gameEntry.Publishers.Add(publisher);

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    if (gameCollection is not null && !gameEntry.GameCollections.Any(gc => gc.Id == gameCollection.Id))
                        gameEntry.GameCollections.Add(gameCollection);

                    return gameEntry;
                },
                new { platformId }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }

    public async Task<IEnumerable<Game>> GetByDeveloperIdAsync(long developerId)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"WITH FilteredGames AS (
    SELECT DISTINCT g.id
    FROM games g
    JOIN gamesdevelopers gd ON gd.gameid = g.id
    WHERE gd.developerId = @developerId
)
SELECT
    g.Id, g.name, g.image, g.releasedate, g.description,
    d.id, d.name,
    p.id, p.name,
    gen.id, gen.name,
    l.id, l.name,
    plat.id, plat.name,
    gs.Id, gs.GameId, gs.ImageUrl,
    gc.Id, gc.Name, gc.Description
FROM games g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN developers d ON d.id = gd.developerid
    LEFT JOIN gamespublishers gpub on gpub.gameid=g.id
    LEFT JOIN publishers p on p.id = gp.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
    LEFT JOIN platforms plat ON plat.id = gp.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
    LEFT JOIN gamescollections gc on gc.gameid = g.id
WHERE g.id IN (SELECT id FROM FilteredGames)
ORDER BY g.id, gen.id;";

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameCollection, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot, gameCollection) =>
                {
                    if (!gameDictionary.TryGetValue(game.Name, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameDictionary.Add(gameEntry.Name, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && !gameEntry.Publishers.Any(p => p.Id == publisher.Id))
                        gameEntry.Publishers.Add(publisher);

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    if (gameCollection is not null && !gameEntry.GameCollections.Any(gc => gc.Id == gameCollection.Id))
                        gameEntry.GameCollections.Add(gameCollection);

                    return gameEntry;
                },
                new { developerId }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }


    public async Task<IEnumerable<Game>> GetByPublisherIdAsync(long publisherId)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"WITH FilteredGames AS (
    SELECT DISTINCT g.id
    FROM games g
    WHERE g.publisherId = @publisherId
)
SELECT
    g.Id, g.name, g.image, g.releasedate, g.description,
    d.id, d.name,
    p.id, p.name,
    gen.id, gen.name,
    l.id, l.name,
    plat.id, plat.name,
    gs.Id, gs.GameId, gs.ImageUrl,
    gc.Id, gc.Name, gc.Description
FROM games g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN developers d ON d.id = gd.developerid
    LEFT JOIN gamespublishers gpub on gpub.gameid=g.id
    LEFT JOIN publishers p on p.id = gp.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
    LEFT JOIN platforms plat ON plat.id = gp.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
    LEFT JOIN gamescollections gc on gc.gameid = g.id
WHERE g.id IN (SELECT id FROM FilteredGames)
ORDER BY g.id, gen.id;";

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameCollection, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot, gameCollection) =>
                {
                    if (!gameDictionary.TryGetValue(game.Name, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameDictionary.Add(gameEntry.Name, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && !gameEntry.Publishers.Any(p => p.Id == publisher.Id))
                        gameEntry.Publishers.Add(publisher);

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    if (gameCollection is not null && !gameEntry.GameCollections.Any(s => s.Id == gameCollection.Id))
                        gameEntry.GameCollections.Add(gameCollection);

                    return gameEntry;
                },
                new { publisherId }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM Games
WHERE Id=@id", new { id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (long id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<Game> UpdateAsync(UpdateGameModel entity, long id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Game>> GetByReleaseYearAsync(int year)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var sql = @"SELECT DISTINCT
    g.Id, g.Name, g.Image, g.ReleaseDate, g.Description,
    d.Id, d.Name,
    p.Id, p.Name,
    gen.Id, gen.Name,
    l.Id, l.Name,
    platf.Id, platf.Name,
    gs.Id, gs.GameId, gs.ImageUrl,
    gc.Id, gc.Name, gc.Description
FROM games g
LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
LEFT JOIN developers d ON d.id = gd.developerid
LEFT JOIN gamespublishers gpub on gpub.gameid = g.id
LEFT JOIN publishers p on p.id = gpub.publisherid
LEFT JOIN gamesgenres gg ON gg.gameid = g.id
LEFT JOIN genres gen ON gen.id = gg.genreid
LEFT JOIN localizations l ON l.id = g.localizationid
LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
LEFT JOIN platforms platf ON platf.id = gplatf.platformid
LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
LEFT JOIN gamescollectionsitems gci ON gci.GameId = g.Id
LEFT JOIN gamescollections gc on gc.Id = gci.GameCollectionId
WHERE EXTRACT(YEAR FROM g.ReleaseDate) = @Year;";

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameCollection, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot, gameCollection) =>
                {
                    if (!gameDictionary.TryGetValue(game.Name, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameEntry.Publishers = new List<Publisher>();
                        gameEntry.GameCollections = new List<GameCollection>();
                        gameDictionary.Add(gameEntry.Name, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && !gameEntry.Publishers.Any(p => p.Id == publisher.Id))
                        gameEntry.Publishers.Add(publisher);

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    if (gameCollection is not null && !gameEntry.GameCollections.Any(b => b.Id == gameCollection.Id))
                        gameEntry.GameCollections.Add(gameCollection);

                    return gameEntry;
                },
                new { Year = year },
                splitOn: "Id,Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values.ToList();

            return result;
        }
    }

    public Task<IEnumerable<Game>> GetAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Game>> GetByParametersAsync(
    long[]? genresIds,
    long[]? platformsIds,
    long[]? years,
    long[]? developersIds,
    long[]? publishersIds,
    int skip,
    int take)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            // Build the base SQL with all joins
            var sqlBuilder = new StringBuilder(@"SELECT DISTINCT
    g.Id, g.Name, g.Image, g.ReleaseDate, g.Description,
    d.Id, d.Name,
    p.Id, p.Name,
    gen.Id, gen.Name,
    l.Id, l.Name,
    platf.Id, platf.Name,
    gs.Id, gs.GameId, gs.ImageUrl,
    gc.Id, gc.Name, gc.Description
FROM games g
LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
LEFT JOIN developers d ON d.id = gd.developerid
LEFT JOIN gamespublishers gpub on gpub.gameid = g.id
LEFT JOIN publishers p on p.id = gpub.publisherid
LEFT JOIN gamesgenres gg ON gg.gameid = g.id
LEFT JOIN genres gen ON gen.id = gg.genreid
LEFT JOIN localizations l ON l.id = g.localizationid
LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
LEFT JOIN platforms platf ON platf.id = gplatf.platformid
LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
LEFT JOIN gamescollectionsitems gci ON gci.GameId = g.Id
LEFT JOIN gamescollections gc on gc.Id = gci.GameCollectionId
WHERE 1=1");

            var parameters = new DynamicParameters();

            // Add filters conditionally
            if (genresIds != null && genresIds.Length > 0)
            {
                sqlBuilder.Append(" AND gg.genreid = ANY(@GenresIds)");
                parameters.Add("GenresIds", genresIds);
            }

            if (platformsIds != null && platformsIds.Length > 0)
            {
                sqlBuilder.Append(" AND gplatf.platformid = ANY(@PlatformsIds)");
                parameters.Add("PlatformsIds", platformsIds);
            }

            if (years != null && years.Length > 0)
            {
                sqlBuilder.Append(" AND EXTRACT(YEAR FROM g.ReleaseDate) = ANY(@Years)");
                parameters.Add("Years", years);
            }

            if (developersIds != null && developersIds.Length > 0)
            {
                sqlBuilder.Append(" AND gd.developerid = ANY(@DevelopersIds)");
                parameters.Add("DevelopersIds", developersIds);
            }

            if (publishersIds != null && publishersIds.Length > 0)
            {
                sqlBuilder.Append(" AND gpub.publisherid = ANY(@PublishersIds)");
                parameters.Add("PublishersIds", publishersIds);
            }

            // Add pagination
            sqlBuilder.Append(" ORDER BY g.Name OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY");
            parameters.Add("Skip", skip);
            parameters.Add("Take", take);

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameCollection, Game>(
                sqlBuilder.ToString(),
                (game, developer, publisher, genre, localization, platform, screenshot, gameCollection) =>
                {
                    if (!gameDictionary.TryGetValue(game.Name, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameEntry.Publishers = new List<Publisher>();
                        gameEntry.GameCollections = new List<GameCollection>();
                        gameDictionary.Add(gameEntry.Name, gameEntry);
                    }

                    if (developer is not null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                        gameEntry.Developers.Add(developer);

                    if (publisher is not null && !gameEntry.Publishers.Any(p => p.Id == publisher.Id))
                        gameEntry.Publishers.Add(publisher);

                    if (genre is not null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                        gameEntry.Genres.Add(genre);

                    if (localization is not null && gameEntry.Localization == null)
                        gameEntry.Localization = localization;

                    if (platform is not null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                        gameEntry.Platforms.Add(platform);

                    if (screenshot is not null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                        gameEntry.Screenshots.Add(screenshot);

                    if (gameCollection is not null && !gameEntry.GameCollections.Any(b => b.Id == gameCollection.Id))
                        gameEntry.GameCollections.Add(gameCollection);

                    return gameEntry;
                },
                parameters,
                splitOn: "Id,Id,Id,Id,Id,Id,Id"
            );

            var result = gameDictionary.Values.ToList();
            return result;
        }
    }
}