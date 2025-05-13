using Dapper;
using Data.Repositories.Interfaces;
using Domain;
using Npgsql;

namespace Data.Repositories.Classes.Derived;

public sealed class LocalizationsRepository : Repository, IRepository<Localization>
{
    public LocalizationsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Localization localization)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Localizations
(Name, Href)
VALUES (@Name, @Href)
RETURNING Id;"
 , new
 {
     localization.Name,
     localization.Href
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
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var localizations = await connection.QueryAsync<Localization>(@"SELECT Id, Name, Href 
FROM 
Localizations;");

            return localizations;
        }
    }

    public async Task<Localization> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var localization = await connection.QueryFirstOrDefaultAsync<Localization>(@"SELECT Id, Name, Href 
FROM 
Localizations
WHERE Id=@id;", new { id });

            if (localization is null)
                return null;

            var localizationGames = await connection.QueryAsync<Game>(@"SELECT Id, Href, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer 
from Games 
where LocalizationId=@localizationId", new { localizationId = localization.Id });

            foreach (var game in localizationGames)
            {
                game.Localization = localization;

                var gamePlatforms = await connection.QueryAsync<GamePlatform>(@"SELECT Id, GameId, PlatformId FROM GamesPlatforms where GameId=@gameId", new { gameId = game.Id });

                var platforms = new List<Platform>();

                foreach (var gamePlatform in gamePlatforms)
                {
                    var platform = await connection.QueryFirstOrDefaultAsync<Platform>(@"SELECT Id, Href, Name 
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
                    var developer = await connection.QueryFirstOrDefaultAsync<Developer>(@"SELECT Id, Name, Url
FROM Developers 
WHERE Id=@Id", new { Id = gameDeveloper.DeveloperId });

                    if (developer is not null)
                    {
                        developers.Add(developer);
                    }
                }

                game.Developers = developers;

                var publisher = await connection.QueryFirstOrDefaultAsync<Publisher>(@"SELECT Id, Name, Url
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
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var localizations = await connection.QueryAsync<Localization>(@"SELECT Id, Name, Href 
FROM 
Localizations 
OFFSET @offset
LIMIT @limit;", new { offset, limit });

            return localizations;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
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
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var updatedLocalization = await connection.QueryFirstOrDefaultAsync(@"UPDATE Localizations set Name=@Name, Href=@Href 
where Id=@Id
returning Name, Href, Id", new
            {
                localization.Name,
                localization.Href,
                id
            });

            return updatedLocalization;
        }
    }
}
