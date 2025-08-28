using Data.Repositories.Interfaces;
using Domain.Games;
using IdentityLibrary.DTOs;
using Data.Extensions;
using Dapper;
using Domain.Reviews;
using Domain.RequestsModels.Games;

namespace Data.Repositories.Classes.Derived.Games;
public sealed class GamesRepository : Repository, IRepository<Game, AddGameModel, UpdateGameModel>
{
    public GamesRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddGameModel entity)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                var insertedGame = await connection.QueryFirstAsync<Game>(@"INSERT INTO Games 
(Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer) 
output inserted.Id, inserted.Name, inserted.Image, inserted.LocalizationId, inserted.PublisherId, inserted.ReleaseDate, inserted.Description, inserted.Trailer
VALUES
(@Name, @Image, @LocalizationId, @PublisherId, @ReleaseDate, @Description, @Trailer);", new
                {
                    entity.Name,
                    entity.Image,
                    entity.LocalizationId,
                    entity.PublisherId,
                    ReleaseDate = entity.ReleaseDate.Value,
                    entity.Description,
                    entity.Trailer
                }, transaction: transaction);

                foreach (var genreId in entity.GenresIds)
                {
                    {
                        var insertedGameGenre = await connection.QueryFirstAsync<GameGenre>(@"INSERT INTO GamesGenres (GameId, GenreId) 
OUTPUT inserted.Id, inserted.GameId, inserted.GenreId
VALUES (@GameId, @GenreId);", new { GameId = insertedGame.Id, GenreId = genreId }, transaction: transaction);
                    }
                }

                foreach (var platformId in entity.PlatformsIds)
                {
                    var insertedGamePlatform = await connection.QueryFirstAsync<GamePlatform>(@"INSERT INTO GamesPlatforms 
(GameId, PlatformId)
output inserted.GameId, inserted.PlatformId
VALUES (@GameId, @PlatformId);", new { GameId = insertedGame.Id, PlatformId = platformId }, transaction: transaction);
                }

                foreach (var developerId in entity.DevelopersIds)
                {
                    var insertedGameDeveloper = await connection.QueryAsync(@"INSERT INTO GamesDevelopers(GameId, DeveloperId)
output inserted.Id, inserted.GameId, inserted.DeveloperId
VALUES(@GameId, @DeveloperId)", new { GameId = insertedGame.Id, DeveloperId = developerId }, transaction: transaction);
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
gs.Id, gs.GameId, gs.ImageUrl
    FROM (select Id, Name, Image, ReleaseDate, Description, PublisherId, LocalizationId 
        from Games ORDER BY id
        OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY) as g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN developers d ON d.id = gd.developerid
    LEFT JOIN publishers p ON p.id = g.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gp ON gp.gameid = g.id
    LEFT JOIN platforms plat ON plat.id = gp.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id";

            var gameDictionary = new Dictionary<long, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
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
                }, new { offset, limit },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values.ToList();

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
gs.id, gs.gameid, gs.imageUrl
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

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
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
gs.Id, gs.GameId, gs.ImageUrl,
gpr.Id, gpr.GameId, gpr.UserId, gpr.Score, gpr.TextContent, gpr.Date,
au.Id, au.UserName, au.NormalizedUserName, au.Email, au.NormalizedEmail, au.EmailConfirmed, au.PasswordHash, au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
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
    LEFT JOIN GamesPlayersReviews gpr on gpr.gameid=g.Id
    LEFT JOIN ApplicationUsers au on au.Id=gpr.UserId
WHERE g.Id=@id";

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameReview, ApplicationUser, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot, gamePlayerReview, applicationUser) =>
                {
                    if (!gameDictionary.TryGetValue(game.Name, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameEntry.GamesPlayersReviews = new List<GameReview>();
                        gameDictionary.Add(gameEntry.Name, gameEntry);
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

                    if (gamePlayerReview is not null && applicationUser is not null)
                    {
                        gamePlayerReview = gamePlayerReview with { ApplicationUser = applicationUser };

                        if (!gameEntry.GamesPlayersReviews.Any(s => s.Id == gamePlayerReview.Id))
                            gameEntry.GamesPlayersReviews.Add(gamePlayerReview);
                    }
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
        using (var connection = new SqlConnection(ConnectionString))
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
    gs.Id, gs.GameId, gs.ImageUrl
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

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
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
                new { genreId }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }

    public async Task<IEnumerable<Game>> GetByPlatformIdAsync(long platformId)
    {
        using (var connection = new SqlConnection(ConnectionString))
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
    gs.Id, gs.GameId, gs.ImageUrl
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

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
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
                new { platformId }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }

    public async Task<IEnumerable<Game>> GetByDeveloperIdAsync(long developerId)
    {
        using (var connection = new SqlConnection(ConnectionString))
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
    gs.Id, gs.GameId, gs.ImageUrl
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

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
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
                new { developerId }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }


    public async Task<IEnumerable<Game>> GetByPublisherIdAsync(long publisherId)
    {
        using (var connection = new SqlConnection(ConnectionString))
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
    gs.Id, gs.GameId, gs.ImageUrl
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

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
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
                new { publisherId }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values;

            return result.ToArray();
        }
    }

    public Task<IEnumerable<Game>> GetAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
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

    public Task<Game> UpdateAsync(UpdateGameModel entity, long id)
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
    WHERE year (g.ReleaseDate) = @year
)
SELECT
    g.Id, g.Name, g.Image, g.ReleaseDate, g.Description,
    d.Id, d.Name,
    p.Id, p.Name,
    gen.Id, gen.Name,
    l.Id, l.Name,
    plat.Id, plat.Name,
    gs.Id, gs.GameId, gs.ImageUrl
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

            var gameDictionary = new Dictionary<string, Game>();

            var query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, Game>(
                sql,
                (game, developer, publisher, genre, localization, platform, screenshot) =>
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
}