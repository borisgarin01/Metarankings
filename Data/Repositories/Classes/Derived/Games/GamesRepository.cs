using Data.Extensions;
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
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        connection.Open();

        using NpgsqlTransaction transaction = connection.BeginTransaction();
        Game insertedGame = await connection.QueryFirstAsync<Game>(@"INSERT INTO Games 
(Name, Image, LocalizationId, ReleaseDate, Description, Trailer) 
VALUES
(@Name, @Image, @LocalizationId, CAST(@ReleaseDate AS DATE), @Description, @Trailer)
RETURNING Id, Name, Image, LocalizationId, ReleaseDate, Description, Trailer;", new
        {
            entity.Name,
            entity.Image,
            entity.LocalizationId,
            ReleaseDate = entity.ReleaseDate.Value,
            entity.Description,
            entity.Trailer
        }, transaction: transaction);

        foreach (long genreId in entity.GenresIds)
        {
            GameGenre insertedGameGenre = await connection.QueryFirstAsync<GameGenre>(@"INSERT INTO GamesGenres (GameId, GenreId) 
VALUES (@GameId, @GenreId)
RETURNING Id, GameId, GenreId;", new { GameId = insertedGame.Id, GenreId = genreId }, transaction: transaction);
        }

        foreach (long publisherId in entity.PublishersIds)
        {
            GamePublisher insertedGamePublisher = await connection.QueryFirstAsync<GamePublisher>(@"INSERT INTO GamesPublishers (GameId, PublisherId) 
VALUES (@GameId, @PublisherId)
RETURNING GameId, PublisherId;", new { GameId = insertedGame.Id, PublisherId = publisherId }, transaction: transaction);
        }

        foreach (long platformId in entity.PlatformsIds)
        {
            GamePlatform insertedGamePlatform = await connection.QueryFirstAsync<GamePlatform>(@"INSERT INTO GamesPlatforms 
(GameId, PlatformId)
VALUES (@GameId, @PlatformId)
RETURNING GameId, PlatformId;", new { GameId = insertedGame.Id, PlatformId = platformId }, transaction: transaction);
        }

        foreach (long developerId in entity.DevelopersIds)
        {
            IEnumerable<dynamic> insertedGameDeveloper = await connection.QueryAsync(@"INSERT INTO GamesDevelopers(GameId, DeveloperId)
VALUES(@GameId, @DeveloperId)
RETURNING Id, GameId, DeveloperId;", new { GameId = insertedGame.Id, DeveloperId = developerId }, transaction: transaction);
        }

        await transaction.CommitAsync();

        return insertedGame.Id;
    }

    public async Task AddRangeAsync(IEnumerable<AddGameModel> games)
    {
        foreach (AddGameModel entity in games)
            await AddAsync(entity);
    }

    public async Task<IEnumerable<Game>> GetFirstAsync(int offset, int limit)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        string sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
plat.id, plat.name, 
gs.Id, gs.GameId, gs.ImageUrl,
gc.Id, gc.Name, gc.Description,
gpr.Id, gpr.GameId, gpr.UserId, gpr.Score, gpr.TextContent, gpr.Date
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
    LEFT JOIN gamescollections gc on gc.Id=gci.GameCollectionId
    LEFT JOIN gamesPlayersReviews gpr on gpr.GameId = g.Id";

        Dictionary<long, Game> gameDictionary = new Dictionary<long, Game>();

        IEnumerable<Game> query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GamesCollection, GameReview, Game>(
            sql,
            (game, developer, publisher, genre, localization, platform, screenshot, gameCollection, gameReview) =>
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

                if (gameReview is not null && !gameEntry.GamesPlayersReviews.Any(gr => gr.Id == gameReview.Id))
                    gameEntry.GamesPlayersReviews.Add(gameReview);

                return gameEntry;
            }, new { offset, limit },
            splitOn: "Id,Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
        );

        List<Game> result = gameDictionary.Values.ToList();

        return result;
    }
    public async Task<IEnumerable<Game>> GetLastAsync(int offset, int limit)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        string sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
plat.id, plat.name, 
gs.Id, gs.GameId, gs.ImageUrl,
gc.Id, gc.Name, gc.Description
    FROM (select Id, Name, Image, ReleaseDate, Description, LocalizationId 
        from Games ORDER BY releasedate desc
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

        Dictionary<long, Game> gameDictionary = new Dictionary<long, Game>();

        IEnumerable<Game> query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GamesCollection, Game>(
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

        List<Game> result = gameDictionary.Values.ToList();

        return result;
    }

    public async Task<IEnumerable<Game>> GetNearestAsync(short limit)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);

        IEnumerable<Game> games = await connection.QueryAsync<Game>(@"WITH future_games AS (
    SELECT Id, Name, Image, LocalizationId, ReleaseDate, Description, Trailer
    FROM Games
    WHERE ReleaseDate >= CURRENT_DATE
    ORDER BY ReleaseDate ASC
    LIMIT @Limit
),
past_games AS (
    SELECT Id, Name, Image, LocalizationId, ReleaseDate, Description, Trailer
    FROM Games
    WHERE ReleaseDate < CURRENT_DATE
    ORDER BY ReleaseDate DESC
    LIMIT @Limit
)
SELECT * FROM future_games
UNION ALL
SELECT * FROM past_games
WHERE NOT EXISTS (SELECT 1 FROM future_games);", new { Limit = limit });

        return games;
    }


    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        string sql = @"SELECT         
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

        Dictionary<string, Game> gameDictionary = new Dictionary<string, Game>();

        IEnumerable<Game> query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GamesCollection, Game>(
            sql,
            (game, developer, publisher, genre, localization, platform, screenshot, gameCollection) =>
            {
                if (!gameDictionary.TryGetValue(game.Name, out Game? gameEntry))
                {
                    gameEntry = game;
                    gameEntry.Developers = new List<Developer>();
                    gameEntry.Genres = new List<Genre>();
                    gameEntry.Platforms = new List<Platform>();
                    gameEntry.Screenshots = new List<GameScreenshot>();
                    gameEntry.Publishers = new List<Publisher>();
                    gameEntry.GameCollections = new List<GamesCollection>();
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

        List<Game> result = gameDictionary.Values.ToList();

        return result;
    }

    public async Task<Game> GetAsync(long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        string sql = @"SELECT         
g.Id, g.name, g.image, g.releasedate, g.description,
d.id, d.name, 
p.id, p.name, 
gen.id, gen.name, 
l.id, l.name,
platf.id, platf.name, 
gs.id, gs.gameid, gs.imageUrl,
gc.Id, gc.Name, gc.Description,
gpr.Id, gpr.GameId, gpr.UserId, gpr.Score, gpr.TextContent, gpr.Date,
gprs.Id, gprs.GamePlayerReviewId, gprs.ShifterId, gprs.Direction,
au.Id, au.UserName, au.NormalizedUserName, au.Email, au.NormalizedEmail, 
au.EmailConfirmed, au.PasswordHash, au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
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
LEFT JOIN GamesPlayersReviewsShifts gprs on gprs.GamePlayerReviewId=gpr.Id
LEFT JOIN ApplicationUsers au on au.Id=gpr.UserId
WHERE g.Id=@id";

        Dictionary<long, Game> gameDictionary = new Dictionary<long, Game>(); // Лучше использовать Id как ключ
        Dictionary<long, GameReview> reviewDictionary = new Dictionary<long, GameReview>(); // Для отслеживания отзывов

        IEnumerable<Game> query = await connection.QueryAsync<Game, Developer, Publisher, Genre,
            Localization, Platform, GameScreenshot, GamesCollection, GameReview,
            GamePlayerReviewShift, ApplicationUser, Game>(
            sql,
            (game, developer, publisher, genre, localization, platform, screenshot,
             gameCollection, gameReview, gamePlayerReviewShift, applicationUser) =>
            {
                // Получаем или создаем Game
                if (!gameDictionary.TryGetValue(game.Id, out Game? gameEntry))
                {
                    gameEntry = game;
                    gameEntry.Developers = new List<Developer>();
                    gameEntry.Publishers = new List<Publisher>();
                    gameEntry.Genres = new List<Genre>();
                    gameEntry.Platforms = new List<Platform>();
                    gameEntry.Screenshots = new List<GameScreenshot>();
                    gameEntry.GameCollections = new List<GamesCollection>();
                    gameEntry.GamesPlayersReviews = new List<GameReview>();
                    gameDictionary.Add(gameEntry.Id, gameEntry);
                }

                // Добавляем Developer
                if (developer?.Id > 0 && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                    gameEntry.Developers.Add(developer);

                // Добавляем Publisher
                if (publisher?.Id > 0 && !gameEntry.Publishers.Any(p => p.Id == publisher.Id))
                    gameEntry.Publishers.Add(publisher);

                // Добавляем Genre
                if (genre?.Id > 0 && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                    gameEntry.Genres.Add(genre);

                // Добавляем Localization
                if (localization?.Id > 0 && gameEntry.Localization == null)
                    gameEntry.Localization = localization;

                // Добавляем Platform
                if (platform?.Id > 0 && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                    gameEntry.Platforms.Add(platform);

                // Добавляем Screenshot
                if (screenshot?.Id > 0 && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                    gameEntry.Screenshots.Add(screenshot);

                // Добавляем GameCollection
                if (gameCollection?.Id > 0 && !gameEntry.GameCollections.Any(gc => gc.Id == gameCollection.Id))
                    gameEntry.GameCollections.Add(gameCollection);

                // ★★★ ИСПРАВЛЕННАЯ ЛОГИКА ДЛЯ REVIEW ★★★
                if (gameReview?.Id > 0 && applicationUser?.Id > 0)
                {
                    // Проверяем, есть ли уже такой отзыв в коллекции
                    GameReview? existingReview = gameEntry.GamesPlayersReviews
                        .FirstOrDefault(r => r.Id == gameReview.Id);

                    if (existingReview == null)
                    {
                        // Создаем новый отзыв с пользователем
                        GameReview newReview = gameReview with
                        {
                            ApplicationUser = applicationUser,
                            GamePlayerReviewShifts = new List<GamePlayerReviewShift>()
                        };

                        // Добавляем сдвиг, если он есть
                        if (gamePlayerReviewShift?.Id > 0)
                        {
                            newReview.GamePlayerReviewShifts.Add(gamePlayerReviewShift);
                        }

                        gameEntry.GamesPlayersReviews.Add(newReview);
                        reviewDictionary[newReview.Id] = newReview;
                    }
                    else
                    {
                        // Добавляем сдвиг к существующему отзыву
                        if (gamePlayerReviewShift?.Id > 0 &&
                            !existingReview.GamePlayerReviewShifts.Any(s => s.Id == gamePlayerReviewShift.Id))
                        {
                            existingReview.GamePlayerReviewShifts.Add(gamePlayerReviewShift);
                        }
                    }
                }

                return gameEntry;
            },
            new { id },
            splitOn: "Id,Id,Id,Id,Id,Id,Id,Id,Id,Id,Id" // Укажите все split точки
        );

        return gameDictionary.Values.FirstOrDefault();
    }

    public async Task RemoveAsync(long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(@"DELETE FROM Games
WHERE Id=@id", new { id });
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

    public Task<IEnumerable<Game>> GetAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Game>> GetByParametersAsync(
    long[]? genresIds,
    long[]? platformsIds,
    int[]? years,
    long[]? developersIds,
    long[]? publishersIds,
    long[]? localizationsIds,
    int skip,
    int take)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);

        // 1. Сначала получаем ID игр, прошедших фильтрацию
        StringBuilder filterSql = new StringBuilder(@"
    SELECT DISTINCT g.Id, g.Name  -- ДОБАВЛЯЕМ g.Name в SELECT
    FROM games g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN gamespublishers gpub ON gpub.gameid = g.id
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
    WHERE 1=1
");

        DynamicParameters parameters = new DynamicParameters();

        // Фильтры для отбора игр
        if (genresIds != null && genresIds.Length > 0)
        {
            filterSql.Append(" AND gg.genreid = ANY(@GenresIds)");
            parameters.Add("GenresIds", genresIds);
        }

        if (platformsIds != null && platformsIds.Length > 0)
        {
            filterSql.Append(" AND gplatf.platformid = ANY(@PlatformsIds)");
            parameters.Add("PlatformsIds", platformsIds);
        }

        if (years != null && years.Length > 0)
        {
            filterSql.Append(" AND EXTRACT(YEAR FROM g.ReleaseDate) = ANY(@Years)");
            parameters.Add("Years", years);
        }

        if (developersIds != null && developersIds.Length > 0)
        {
            filterSql.Append(" AND gd.developerid = ANY(@DevelopersIds)");
            parameters.Add("DevelopersIds", developersIds);
        }

        if (publishersIds != null && publishersIds.Length > 0)
        {
            filterSql.Append(" AND gpub.publisherid = ANY(@PublishersIds)");
            parameters.Add("PublishersIds", publishersIds);
        }

        if (localizationsIds != null && localizationsIds.Length > 0)
        {
            filterSql.Append(" AND g.localizationid = ANY(@LocalizationsIds)");
            parameters.Add("LocalizationsIds", localizationsIds);
        }

        // Пагинация применяется к ID
        filterSql.Append(" ORDER BY g.Name OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY");
        parameters.Add("Skip", skip);
        parameters.Add("Take", take);

        // Получаем ID и Name игр (исправлено с QueryAsync<long> на QueryAsync<(long Id, string Name)>)
        var gameIdsWithNames = await connection.QueryAsync<(long Id, string Name)>(filterSql.ToString(), parameters);

        if (!gameIdsWithNames.Any())
            return Enumerable.Empty<Game>();

        // Извлекаем только ID для второго запроса
        var gameIds = gameIdsWithNames.Select(x => x.Id).ToArray();

        // 2. Теперь загружаем ПОЛНЫЕ данные по этим играм (ВСЕ платформы, жанры и т.д.)
        string dataSql = @"
    SELECT 
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
    LEFT JOIN gamespublishers gpub ON gpub.gameid = g.id
    LEFT JOIN publishers p ON p.id = gpub.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
    LEFT JOIN platforms platf ON platf.id = gplatf.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id
    LEFT JOIN gamescollectionsitems gci ON gci.GameId = g.Id
    LEFT JOIN gamescollections gc ON gc.Id = gci.GameCollectionId
    WHERE g.Id = ANY(@GameIds)
    ORDER BY g.Name";

        parameters.Add("GameIds", gameIds);

        Dictionary<long, Game> gameDictionary = new Dictionary<long, Game>();

        IEnumerable<Game> query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GamesCollection, Game>(
            dataSql,
            (game, developer, publisher, genre, localization, platform, screenshot, gameCollection) =>
            {
                if (!gameDictionary.TryGetValue(game.Id, out Game? gameEntry))
                {
                    gameEntry = game;
                    gameEntry.Developers = new List<Developer>();
                    gameEntry.Genres = new List<Genre>();
                    gameEntry.Platforms = new List<Platform>();
                    gameEntry.Screenshots = new List<GameScreenshot>();
                    gameEntry.Publishers = new List<Publisher>();
                    gameEntry.GameCollections = new List<GamesCollection>();
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
            },
            parameters,
            splitOn: "Id,Id,Id,Id,Id,Id,Id"
        );

        return gameDictionary.Values.ToList();
    }

    public async Task<IEnumerable<Game>> GetByNameAsync(string name)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        string sql = @"SELECT         
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
WHERE g.name ILIKE '%' || @name || '%'
        ORDER BY g.Id DESC;";

        Dictionary<string, Game> gameDictionary = new Dictionary<string, Game>();

        IEnumerable<Game> query = await connection.QueryAsync<Game, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GamesCollection, GameReview, ApplicationUser, Game>(
            sql,
            (game, developer, publisher, genre, localization, platform, screenshot, gameCollection, gamePlayerReview, applicationUser) =>
            {
                if (!gameDictionary.TryGetValue(game.Name, out Game? gameEntry))
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
            new { name } // Parameter passed here
        );

        return gameDictionary.Values;
    }

    public async Task<int> GetCountByParametersAsync(
    long[]? genresIds,
    long[]? platformsIds,
    int[]? years,
    long[]? developersIds,
    long[]? publishersIds,
    long[]? localizationsIds)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);

        StringBuilder countSql = new StringBuilder(@"
        SELECT COUNT(DISTINCT g.Id)
        FROM games g
        LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
        LEFT JOIN gamespublishers gpub ON gpub.gameid = g.id
        LEFT JOIN gamesgenres gg ON gg.gameid = g.id
        LEFT JOIN gamesplatforms gplatf ON gplatf.gameid = g.id
        WHERE 1=1
    ");

        DynamicParameters parameters = new DynamicParameters();

        // Применяем те же фильтры, что и в GetByParametersAsync
        if (genresIds != null && genresIds.Length > 0)
        {
            countSql.Append(" AND gg.genreid = ANY(@GenresIds)");
            parameters.Add("GenresIds", genresIds);
        }

        if (platformsIds != null && platformsIds.Length > 0)
        {
            countSql.Append(" AND gplatf.platformid = ANY(@PlatformsIds)");
            parameters.Add("PlatformsIds", platformsIds);
        }

        if (years != null && years.Length > 0)
        {
            countSql.Append(" AND EXTRACT(YEAR FROM g.ReleaseDate) = ANY(@Years)");
            parameters.Add("Years", years);
        }

        if (developersIds != null && developersIds.Length > 0)
        {
            countSql.Append(" AND gd.developerid = ANY(@DevelopersIds)");
            parameters.Add("DevelopersIds", developersIds);
        }

        if (publishersIds != null && publishersIds.Length > 0)
        {
            countSql.Append(" AND gpub.publisherid = ANY(@PublishersIds)");
            parameters.Add("PublishersIds", publishersIds);
        }

        if (localizationsIds != null && localizationsIds.Length > 0)
        {
            countSql.Append(" AND g.localizationid = ANY(@LocalizationsIds)");
            parameters.Add("LocalizationsIds", localizationsIds);
        }

        int totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString(), parameters);
        return totalCount;
    }
}