using Dapper;
using Data.Repositories.Interfaces;
using Domain;
using Npgsql;

namespace Data.Repositories.Classes.Derived;
public sealed class GamesRepository : Repository, IRepository<GameModel>
{
    public GamesRepository(string connectionString) : base(connectionString)
    {
    }

    public Task<long> AddAsync(GameModel entity)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(IEnumerable<GameModel> entities)
    {
        throw new NotImplementedException();
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
                new { id }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = gameDictionary.Values.FirstOrDefault();

            return result;
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
}
