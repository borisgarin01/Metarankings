using Dapper;
using Domain;
using Npgsql;

namespace Data.Repositories.Classes.Derived;
public sealed class GamesRepository : Repository
{
    public GamesRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<IEnumerable<GameModel>> GetGameModels()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var gameDictionary = new Dictionary<long, GameModel>();

            var games = await connection.QueryAsync<GameModel, Developer, Publisher, Genre, Localization, Platform, GameScreenshot, GameModel>(
                @"SELECT 
        *
    FROM games g
    LEFT JOIN gamesdevelopers gd ON gd.gameid = g.id
    LEFT JOIN developers d ON d.id = gd.developerid
    LEFT JOIN publishers p ON p.id = g.publisherid
    LEFT JOIN gamesgenres gg ON gg.gameid = g.id
    LEFT JOIN genres gen ON gen.id = gg.genreid
    LEFT JOIN localizations l ON l.id = g.localizationid
    LEFT JOIN gamesplatforms gp ON gp.gameid = g.id
    LEFT JOIN platforms plat ON plat.id = gp.platformid
    LEFT JOIN gamesscreenshots gs ON gs.gameid = g.id",
                (game, developer, publisher, genre, localization, platform, screenshot) =>
                {
                    if (!gameDictionary.TryGetValue(game.Id, out var gameEntry))
                    {
                        gameEntry = game;
                        gameEntry.Developers = new List<Developer>();
                        gameEntry.Genres = new List<Genre>();
                        gameEntry.Platforms = new List<Platform>();
                        gameEntry.Screenshots = new List<GameScreenshot>();
                        gameEntry.Publisher = publisher;
                        gameEntry.Localization = localization;
                        gameDictionary.Add(gameEntry.Id, gameEntry);
                    }

                    if (developer != null && !gameEntry.Developers.Any(d => d.Id == developer.Id))
                    {
                        ((List<Developer>)gameEntry.Developers).Add(developer);
                    }

                    if (genre != null && !gameEntry.Genres.Any(g => g.Id == genre.Id))
                    {
                        ((List<Genre>)gameEntry.Genres).Add(genre);
                    }

                    if (platform != null && !gameEntry.Platforms.Any(p => p.Id == platform.Id))
                    {
                        ((List<Platform>)gameEntry.Platforms).Add(platform);
                    }

                    if (screenshot != null && !gameEntry.Screenshots.Any(s => s.Id == screenshot.Id))
                    {
                        ((List<GameScreenshot>)gameEntry.Screenshots).Add(screenshot);
                    }

                    return gameEntry;
                },
                splitOn: "id,id,id,id,id,id");

            return games;
        }
    }
}
