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
            var localizations = await connection.QueryAsync<Localization>(@"SELECT Id, Name
FROM 
Localizations;");

            return localizations;
        }
    }

    public async Task<Localization> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localization = await connection.QueryFirstOrDefaultAsync<Localization>(@"SELECT Id, Name 
FROM 
Localizations
WHERE Id=@id;", new { id });

            if (localization is null)
                return null;

            var localizationGames = await connection.QueryAsync<Game>(@"SELECT Id, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer 
from Games 
where LocalizationId=@localizationId", new { localizationId = localization.Id });

            foreach (var game in localizationGames)
            {
                game.Localization = localization;

                var gamePlatforms = await connection.QueryAsync<GamePlatform>(@"SELECT Id, GameId, PlatformId FROM GamesPlatforms where GameId=@gameId", new { gameId = game.Id });

                var platforms = new List<Platform>();

                foreach (var gamePlatform in gamePlatforms)
                {
                    var platform = await connection.QueryFirstOrDefaultAsync<Platform>(@"SELECT Id, Name 
FROM Platforms
WHERE Id=@Id", new { Id = gamePlatform.PlatformId });

                    if (platform is not null)
                    {
                        platforms.Add(platform);
                    }
                }

                game.Platforms = platforms;

                var gameDevelopers = await connection.QueryAsync<DeveloperGame>(@"SELECT Id, GameId, DeveloperId FROM GamesDevelopers 
WHERE GameId=@GameId", new { GameId = game.Id });

                var developers = new List<Developer>();

                foreach (var gameDeveloper in gameDevelopers)
                {
                    var developer = await connection.QueryFirstOrDefaultAsync<Developer>(@"SELECT Id, Name
FROM Developers 
WHERE Id=@Id", new { Id = gameDeveloper.DeveloperId });

                    if (developer is not null)
                    {
                        developers.Add(developer);
                    }
                }

                game.Developers = developers;

                var publisher = await connection.QueryFirstOrDefaultAsync<Publisher>(@"SELECT Id, Name
FROM Publishers
WHERE Id=@Id", new { Id = game.PublisherId });

                if (publisher is not null)
                {
                    game.Publisher = publisher;
                }
            }

            localization.Games = localizationGames;

            return localization;
        }
    }

    public async Task<Localization> GetByPlatformAsync(long id, long platformId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localization = await connection.QueryFirstOrDefaultAsync<Localization>(@"SELECT Id, Name 
FROM 
Localizations
WHERE Id=@id;", new { id });

            if (localization is null)
                return null;

            var localizationGames = await connection.QueryAsync<Game>(@"SELECT Id, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer 
from Games 
where LocalizationId=@localizationId and 
Id in (select GameId from GamesPlatforms where PlatformId=@platformId)", new { localizationId = localization.Id, platformId });

            foreach (var game in localizationGames)
            {
                game.Localization = localization;

                var gamePlatforms = await connection.QueryAsync<GamePlatform>(@"SELECT Id, GameId, PlatformId FROM GamesPlatforms where GameId=@gameId", new { gameId = game.Id });

                var platforms = new List<Platform>();

                foreach (var gamePlatform in gamePlatforms)
                {
                    var platform = await connection.QueryFirstOrDefaultAsync<Platform>(@"SELECT Id, Name 
FROM Platforms
WHERE Id=@Id", new { Id = gamePlatform.PlatformId });

                    if (platform is not null)
                    {
                        platforms.Add(platform);
                    }
                }

                game.Platforms = platforms;

                var gameDevelopers = await connection.QueryAsync<DeveloperGame>(@"SELECT Id, GameId, DeveloperId FROM GamesDevelopers 
WHERE GameId=@GameId", new { GameId = game.Id });

                var developers = new List<Developer>();

                foreach (var gameDeveloper in gameDevelopers)
                {
                    var developer = await connection.QueryFirstOrDefaultAsync<Developer>(@"SELECT Id, Name
FROM Developers 
WHERE Id=@Id", new { Id = gameDeveloper.DeveloperId });

                    if (developer is not null)
                    {
                        developers.Add(developer);
                    }
                }

                game.Developers = developers;

                var publisher = await connection.QueryFirstOrDefaultAsync<Publisher>(@"SELECT Id, Name
FROM Publishers
WHERE Id=@Id", new { Id = game.PublisherId });

                if (publisher is not null)
                {
                    game.Publisher = publisher;
                }
            }

            localization.Games = localizationGames;

            return localization;
        }
    }

    public async Task<IEnumerable<Localization>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localizations = await connection.QueryAsync<Localization>(@"SELECT Id, Name 
FROM 
Localizations 
OFFSET @offset
LIMIT @limit;", new { offset, limit });

            return localizations;
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
