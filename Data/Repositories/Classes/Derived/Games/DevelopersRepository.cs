using Dapper;
using Data.Repositories.Interfaces;
using Domain.Games;
using Microsoft.Data.SqlClient;

namespace Data.Repositories.Classes.Derived.Games;
public sealed class DevelopersRepository : Repository, IRepository<Developer>
{
    public DevelopersRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Developer developer)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Developers
(Name)
OUTPUT inserted.Id
VALUES (@Name);"
 , new
 {
     developer.Name
 });
            return id;
        }
    }

    public async Task AddRangeAsync(IEnumerable<Developer> developers)
    {
        foreach (var developer in developers)
        {
            await AddAsync(developer);
        }
    }

    public async Task<IEnumerable<Developer>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var developers = await connection.QueryAsync<Developer>(@"select 
developers.id, developers.name from developers");

            if (developers is null)
                return null;

            if (!developers.Any())
                return developers;

            foreach (var developer in developers)
            {
                var developerGames = await connection.QueryAsync<DeveloperGame>(@"SELECT Id, GameId, DeveloperId 
from GamesDevelopers 
where developerId=@developerId", new { developerId = developer.Id });

                var games = new List<Game>();

                foreach (var gameDeveloper in developerGames)
                {
                    var game = await connection.QueryFirstOrDefaultAsync<Game>(@"SELECT Id, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer
FROM Games WHERE Id=@GameId", new { gameDeveloper.GameId });

                    if (games is not null)
                        games.Add(game);
                }

                foreach (var game in games)
                {
                    var platforms = await connection.QueryAsync<Platform>(@"SELECT platforms.Id, platforms.Name
FROM
platforms
INNER JOIN gamesPlatforms
on platforms.Id=gamesPlatforms.PlatformId
INNER JOIN games
on games.Id=gamesPlatforms.GameId
WHERE gamesPlatforms.GameId=@GameId", new { GameId = game.Id });

                    if (platforms is not null)
                        game.Platforms = platforms.ToList();
                    else
                        game.Platforms = new();

                    var genres = await connection.QueryAsync<Genre>(@"SELECT genres.Id, genres.Name 
FROM
genres
INNER JOIN gamesGenres
on genres.Id=gamesGenres.GenreId
INNER JOIN games
on games.Id=gamesGenres.GameId
WHERE gamesGenres.GameId=@GameId", new { GameId = game.Id });

                    if (genres is not null)
                        game.Genres = genres.ToList();
                    else
                        game.Genres = new();
                }
                developer.Games = games;
            }
            return developers;
        }
    }

    public async Task<Developer> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var developer = await connection.QueryFirstOrDefaultAsync<Developer>(@"select 
developers.id, developers.name from developers
WHERE developers.Id=@id", new { id });

            if (developer is null)
                return null;

            var developerGames = await connection.QueryAsync<DeveloperGame>(@"SELECT Id, GameId, DeveloperId 
from GamesDevelopers 
where developerId=@developerId", new { developerId = developer.Id });

            foreach (var gameDeveloper in developerGames)
            {
                var games = await connection.QueryAsync<Game>(@"SELECT Id, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer
FROM Games WHERE Id=@GameId", new { gameDeveloper.GameId });

                developer.Games.AddRange(games);
            }

            foreach (var game in developer.Games)
            {
                var platforms = await connection.QueryAsync<Platform>(@"SELECT platforms.Id, platforms.Name
FROM
platforms
INNER JOIN gamesPlatforms
on platforms.Id=gamesPlatforms.PlatformId
INNER JOIN games
on games.Id=gamesPlatforms.GameId
WHERE gamesPlatforms.GameId=@GameId", new { GameId = game.Id });

                game.Platforms = platforms.ToList();

                var genres = await connection.QueryAsync<Genre>(@"SELECT genres.Id, genres.Name 
FROM
genres
INNER JOIN gamesGenres
on genres.Id=gamesGenres.GenreId
INNER JOIN games
on games.Id=gamesGenres.GameId
WHERE gamesGenres.GameId=@GameId", new { GameId = game.Id });

                game.Genres = genres.ToList();
            }

            return developer;
        }
    }

    public async Task<IEnumerable<Developer>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var developers = await connection.QueryAsync<Developer>(@"select 
developers.id, developers.name 
from developers
OFFSET @offset limit @limit", new { offset, limit });

            if (developers is null)
                return null;

            if (!developers.Any())
                return null;

            foreach (var developer in developers)
            {

                var developerGames = await connection.QueryAsync<DeveloperGame>(@"SELECT Id, GameId, DeveloperId 
from GamesDevelopers 
where developerId=@developerId", new { developerId = developer.Id });

                foreach (var gameDeveloper in developerGames)
                {
                    var games = await connection.QueryAsync<Game>(@"SELECT Id, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer
FROM Games WHERE Id=@GameId", new { gameDeveloper.GameId });

                    developer.Games.AddRange(games);
                }


                foreach (var game in developer.Games)
                {
                    var platforms = await connection.QueryAsync<Platform>(@"SELECT platforms.Id, platforms.Name
FROM
platforms
INNER JOIN gamesPlatforms
on platforms.Id=gamesPlatforms.PlatformId
INNER JOIN games
on games.Id=gamesPlatforms.GameId
WHERE gamesPlatforms.GameId=@GameId", new { GameId = game.Id });

                    game.Platforms.AddRange(platforms);

                    var genres = await connection.QueryAsync<Genre>(@"SELECT genres.Id, genres.Name 
FROM
genres
INNER JOIN gamesGenres
on genres.Id=gamesGenres.GenreId
INNER JOIN games
on games.Id=gamesGenres.GameId
WHERE gamesGenres.GameId=@GameId", new { GameId = game.Id });

                    game.Genres.AddRange(genres);
                }
            }
            return developers;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM 
Developers WHERE Id=@id", new { id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<Developer> UpdateAsync(Developer developer, long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var updatedDeveloper = await connection.QueryFirstOrDefaultAsync<Developer>(@"UPDATE Developers set Name=@Name
OUTPUT inserted.Name, inserted.Id
where Id=@id", new
            {
                developer.Name,
                id
            });

            return updatedDeveloper;
        }
    }
}
