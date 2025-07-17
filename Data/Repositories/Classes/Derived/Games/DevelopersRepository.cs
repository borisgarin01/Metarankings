using Data.Repositories.Interfaces;
using Domain.Games;

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
            var id = await connection.QueryFirstAsync<long>(@"
INSERT INTO Developers
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
            var developers = await connection.QueryAsync<Developer, Game, Publisher, Platform, Developer>(@"SELECT Developers.Id, Developers.Name,
Games.Id, Games.Name, Games.Image,
Publishers.Id, Publishers.Name,
Platforms.Id, Platforms.Name
FROM Developers
LEFT JOIN 
GamesDevelopers 
    on Developers.Id=GamesDevelopers.DeveloperID
LEFT JOIN Games 
    ON Games.Id=GamesDevelopers.GameId
LEFT JOIN Publishers
    ON Publishers.Id=Games.PublisherId
LEFT JOIN GamesPlatforms
    ON GamesPlatforms.GameId=Games.Id
LEFT JOIN Platforms
    ON GamesPlatforms.PlatformId=Platforms.Id", (developer, game, publisher, platform) =>
            {
                if (game is not null)
                {
                    if (publisher is not null)
                    {
                        game = game with { Publisher = publisher };
                    }

                    game.Platforms.Add(platform);
                    developer.Games.Add(game);
                }
                return developer;
            });

            var developersResult = developers.GroupBy(p => p.Id).Select(g =>
            {
                var groupedDeveloper = g.First();

                groupedDeveloper = groupedDeveloper with
                {
                    Games = g.SelectMany(d => d.Games)
                    .GroupBy(g => g.Id)
                    .Select(gameGroup =>
                    {
                        var game = gameGroup.First();
                        game = game with
                        {
                            Platforms = gameGroup.SelectMany(g => g.Platforms)
                        .DistinctBy(p => p.Id).ToList()
                        };
                        return game;
                    }).ToList()
                };

                return groupedDeveloper;
            });

            return developersResult;
        }
    }

    public async Task<Developer> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var developers = await connection.QueryAsync<Developer, Game, Publisher, Platform, Developer>(@"SELECT Developers.Id, Developers.Name,
Games.Id, Games.Name, Games.Image,
Publishers.Id, Publishers.Name,
Platforms.Id, Platforms.Name
FROM Developers
LEFT JOIN 
GamesDevelopers 
    on Developers.Id=GamesDevelopers.DeveloperID
LEFT JOIN Games 
    ON Games.Id=GamesDevelopers.GameId
LEFT JOIN Publishers
    ON Publishers.Id=Games.PublisherId
LEFT JOIN GamesPlatforms
    ON GamesPlatforms.GameId=Games.Id
LEFT JOIN Platforms
    ON GamesPlatforms.PlatformId=Platforms.Id
WHERE Developers.Id=@id", (developer, game, publisher, platform) =>
            {
                if (game is not null)
                {
                    if (publisher is not null)
                    {
                        game = game with { Publisher = publisher };
                    }

                    game.Platforms.Add(platform);
                    developer.Games.Add(game);
                }
                return developer;
            }, new { id });

            var developersResult = developers.GroupBy(p => p.Id).Select(g =>
            {
                var groupedDeveloper = g.First();

                groupedDeveloper = groupedDeveloper with
                {
                    Games = g.SelectMany(d => d.Games)
                    .GroupBy(g => g.Id)
                    .Select(gameGroup =>
                    {
                        var game = gameGroup.First();
                        game = game with
                        {
                            Platforms = gameGroup.SelectMany(g => g.Platforms)
                        .DistinctBy(p => p.Id).ToList()
                        };
                        return game;
                    }).ToList()
                };

                return groupedDeveloper;
            });

            return developersResult.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Developer>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            IEnumerable<Developer> developers = await connection.QueryAsync<Developer, Game, Platform, Genre, Developer>(@"
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
                if (game is not null)
                {
                    if (platform is not null)
                        game.Platforms.Add(platform);

                    if (genre is not null)
                        game.Genres.Add(genre);

                    developer.Games.Add(game);
                }
                return developer;
            }, new { offset, limit });

            IEnumerable<Developer> developersResult = developers.GroupBy(p => p.Id).Select(dg =>
            {
                Developer groupedDeveloper = dg.First();

                groupedDeveloper = groupedDeveloper with
                {
                    Games = dg.SelectMany(d => d.Games)
                    .GroupBy(g => g.Id)
                    .Select(gameGroup =>
                    {
                        var game = gameGroup.First();
                        game = game with
                        {
                            Platforms = gameGroup.SelectMany(g => g.Platforms)
                        .DistinctBy(p => p.Id).ToList()
                        };
                        return game;
                    }).ToList()
                };

                return groupedDeveloper;
            });

            return developersResult;
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
