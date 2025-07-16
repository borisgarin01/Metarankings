using Domain.Games;
using Data.Repositories.Interfaces;
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
            var developersDictionary = new Dictionary<long, Developer>();

            var developers = await connection.QueryAsync<Developer, Game, Platform, Genre, Developer>(@"
select developers.id, developers.name,
	games.Id, games.Name, games.Image, Games.LocalizationId, 
	Games.PublisherId, Games.ReleaseDate, Games.Description,
	Games.Trailer,
	Platforms.Id, Platforms.Name,
	Genres.Id, Genres.Name
from developers
	left join GamesDevelopers
		on GamesDevelopers.DeveloperId=Developers.Id
	left join Games 
		on Games.Id=GamesDevelopers.GameId
	left join GamesPlatforms
		on GamesPlatforms.GameId=Games.Id
	left join Platforms
		on Platforms.Id=GamesPlatforms.Id
	left join GamesGenres
		on GamesGenres.GameId=Games.Id
	left join Genres
		on Genres.Id=GamesGenres.GenreId", (developer, game, platform, genre) =>
            {
                if (!game.Platforms.Any(b => b.Id == platform.Id))
                    game.Platforms.Add(platform);
                if (!game.Genres.Any(b => b.Id == genre.Id))
                    game.Genres.Add(genre);
                if (!developer.Games.Any(b => b.Id == game.Id))
                    developer.Games.Add(game);

                if (!developersDictionary.TryGetValue(developer.Id, out var dev))
                    developersDictionary.Add(developer.Id, developer);
                return developer;
            });
            return developersDictionary.Values;
        }
    }

    public async Task<Developer> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var sql = @"select developers.id, developers.name,
	games.Id, games.Name, games.Image, Games.LocalizationId, 
	Games.PublisherId, Games.ReleaseDate, Games.Description,
	Games.Trailer,
	Platforms.Id, Platforms.Name,
	Genres.Id, Genres.Name
from developers
	left join GamesDevelopers
		on GamesDevelopers.DeveloperId=Developers.Id
	left join Games 
		on Games.Id=GamesDevelopers.GameId
	left join GamesPlatforms
		on GamesPlatforms.GameId=Games.Id
	left join Platforms
		on Platforms.Id=GamesPlatforms.Id
	left join GamesGenres
		on GamesGenres.GameId=Games.Id
	left join Genres
		on Genres.Id=GamesGenres.GenreId
    WHERE Developers.Id=@id";

            var developersDictionary = new Dictionary<long, Developer>();

            var query = await connection.QueryAsync<Developer, Game, Platform, Genre, Developer>(
                sql,
                (developer, game, platform, genre) =>
                {
                    if (!developersDictionary.TryGetValue(developer.Id, out var developerEntry))
                    {
                        developerEntry = developer;
                        developerEntry.Games = new List<Game>();
                        developersDictionary.Add(developer.Id, developerEntry);
                    }

                    if (game is not null && !developerEntry.Games.Any(d => d.Id == game.Id))
                        developerEntry.Games.Add(game);

                    if (genre is not null && !game.Genres.Any(g => g.Id == genre.Id))
                        game.Genres.Add(genre);

                    if (platform is not null && !game.Platforms.Any(p => p.Id == platform.Id))
                        game.Platforms.Add(platform);

                    developerEntry.Games.Add(game);

                    return developerEntry;
                },
                new { id }, // Parameter passed here
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = developersDictionary.Values.FirstOrDefault();

            return result;
        }
    }

    public async Task<IEnumerable<Developer>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var developersDictionary = new Dictionary<long, Developer>();

            var developers = await connection.QueryAsync<Developer, Game, Platform, Genre, Developer>(@"
select developers.id, developers.name,
	games.Id, games.Name, games.Image, Games.LocalizationId, 
	Games.PublisherId, Games.ReleaseDate, Games.Description,
	Games.Trailer,
	Platforms.Id, Platforms.Name,
	Genres.Id, Genres.Name
from developers
	left join GamesDevelopers
		on GamesDevelopers.DeveloperId=Developers.Id
	left join Games 
		on Games.Id=GamesDevelopers.GameId
	left join GamesPlatforms
		on GamesPlatforms.GameId=Games.Id
	left join Platforms
		on Platforms.Id=GamesPlatforms.Id
	left join GamesGenres
		on GamesGenres.GameId=Games.Id
	left join Genres
		on Genres.Id=GamesGenres.GenreId
    WHERE Developers.Id IN 
        (SELECT Developers.id 
            FROM Developers
            ORDER BY Id ASC
            OFFSET @offset ROWS
            FETCH NEXT @limit ROWS ONLY);", (developer, game, platform, genre) =>
            {
                if (!developer.Games.Any(g => g.Id == game.Id))
                    developer.Games.Add(game);
                if (!game.Genres.Any(b => b.Id == genre.Id))
                    game.Genres.Add(genre);
                if (!developer.Games.Any(b => b.Id == game.Id))
                    developer.Games.Add(game);

                if (!developersDictionary.TryGetValue(developer.Id, out var dev))
                    developersDictionary.Add(developer.Id, developer);
                return developer;
            }, new { offset, limit });
            return developersDictionary.Values;
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
