using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using System;

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
            var localizations = await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(@"
SELECT Localizations.Id, Localizations.Name,
	Games.Id, Games.Name, Games.Image, Games.ReleaseDate, 
		Games.Description, Games.Trailer,
	Platforms.Id, Platforms.Name,
	Developers.Id, Developers.Name,
	Publishers.Id, Publishers.Name
FROM Localizations
left join Games 
	on Localizations.Id=Games.LocalizationId
left join GamesPlatforms
	on GamesPlatforms.GameId=Games.Id
left join Platforms
	on Platforms.Id=GamesPlatforms.PlatformId
left join GamesDevelopers
	on GamesDevelopers.GameId=Games.Id
left join Developers
	on GamesDevelopers.DeveloperId=Developers.Id
left join Publishers
	on Games.PublisherId=Publishers.Id;",
    (localization, game, platform, developer, publisher) =>
    {
        if (game is not null)
        {
            if (platform is not null)
                game.Platforms.Add(platform);
            if (developer is not null)
                game.Developers.Add(developer);
            if (publisher is not null)
                game = game with { Publisher = publisher };

            localization.Games.Add(game);
        }
        return localization;
    });

            var localizationsResult = localizations
                .GroupBy(d => d.Id)
                .Select(g =>
                {
                    Localization groupedLocalization = g.First() with
                    {
                        Games = g.SelectMany(d => d.Games)
                        .GroupBy(g => g.Id)
                        .Select(gameGroup =>
                        {
                            var game = gameGroup.First();
                            game = game with
                            {
                                Platforms = gameGroup.SelectMany(g => g.Platforms)
                            .DistinctBy(p => p.Id).ToList(),
                                Developers = gameGroup.SelectMany(g => g.Developers)
                                .DistinctBy(p => p.Id).ToList()
                            };
                            return game;
                        }).ToList()
                    };

                    return groupedLocalization;
                });

            return localizationsResult;
        }
    }

    public async Task<Localization> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localizations = await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(@"
SELECT Localizations.Id, Localizations.Name,
	Games.Id, Games.Name, Games.Image, Games.ReleaseDate, 
		Games.Description, Games.Trailer,
	Platforms.Id, Platforms.Name,
	Developers.Id, Developers.Name,
	Publishers.Id, Publishers.Name
FROM Localizations
left join Games 
	on Localizations.Id=Games.LocalizationId
left join GamesPlatforms
	on GamesPlatforms.GameId=Games.Id
left join Platforms
	on Platforms.Id=GamesPlatforms.PlatformId
left join GamesDevelopers
	on GamesDevelopers.GameId=Games.Id
left join Developers
	on GamesDevelopers.DeveloperId=Developers.Id
left join Publishers
	on Games.PublisherId=Publishers.Id
WHERE Localizations.Id=@id;",
    (localization, game, platform, developer, publisher) =>
    {
        if (game is not null)
        {
            if (platform is not null)
                game.Platforms.Add(platform);
            if (developer is not null)
                game.Developers.Add(developer);
            if (publisher is not null)
                game = game with { Publisher = publisher };

            localization.Games.Add(game);
        }
        return localization;
    }, new { id });

            var localizationsResult = localizations
                .GroupBy(d => d.Id)
                .Select(g =>
                {
                    Localization groupedLocalization = g.First() with
                    {
                        Games = g.SelectMany(d => d.Games)
                        .GroupBy(g => g.Id)
                        .Select(gameGroup =>
                        {
                            var game = gameGroup.First();
                            game = game with
                            {
                                Platforms = gameGroup.SelectMany(g => g.Platforms)
                            .DistinctBy(p => p.Id).ToList(),
                                Developers = gameGroup.SelectMany(g => g.Developers)
                                .DistinctBy(p => p.Id).ToList()
                            };
                            return game;
                        }).ToList()
                    };

                    return groupedLocalization;
                });

            return localizationsResult.FirstOrDefault();
        }
    }

    public async Task<Localization> GetByPlatformAsync(long id, long platformId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localizations = await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(@"
SELECT Localizations.Id, Localizations.Name,
	Games.Id, Games.Name, Games.Image, Games.ReleaseDate, 
		Games.Description, Games.Trailer,
	Platforms.Id, Platforms.Name,
	Developers.Id, Developers.Name,
	Publishers.Id, Publishers.Name
FROM Localizations
left join Games 
	on Localizations.Id=Games.LocalizationId
left join GamesPlatforms
	on GamesPlatforms.GameId=Games.Id
left join Platforms
	on Platforms.Id=GamesPlatforms.PlatformId
left join GamesDevelopers
	on GamesDevelopers.GameId=Games.Id
left join Developers
	on GamesDevelopers.DeveloperId=Developers.Id
left join Publishers
	on Games.PublisherId=Publishers.Id
WHERE Games.LocalizationId=@Id
    AND Platforms.Id=@PlatformId",
    (localization, game, platform, developer, publisher) =>
    {
        if (game is not null)
        {
            if (platform is not null)
                game.Platforms.Add(platform);
            if (developer is not null)
                game.Developers.Add(developer);
            if (publisher is not null)
                game = game with { Publisher = publisher };

            localization.Games.Add(game);
        }
        return localization;
    }, new { Id = id, PlatformId = platformId });

            var localizationsResult = localizations
                .GroupBy(d => d.Id)
                .Select(g =>
                {
                    Localization groupedLocalization = g.First() with
                    {
                        Games = g.SelectMany(d => d.Games)
                        .GroupBy(g => g.Id)
                        .Select(gameGroup =>
                        {
                            var game = gameGroup.First();
                            game = game with
                            {
                                Platforms = gameGroup.SelectMany(g => g.Platforms)
                            .DistinctBy(p => p.Id).ToList(),
                                Developers = gameGroup.SelectMany(g => g.Developers)
                                .DistinctBy(p => p.Id).ToList()
                            };
                            return game;
                        }).ToList()
                    };

                    return groupedLocalization;
                });

            return localizations.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Localization>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localizations = await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(@"
SELECT Localizations.Id, Localizations.Name,
	Games.Id, Games.Name, Games.Image, Games.ReleaseDate, 
		Games.Description, Games.Trailer,
	Platforms.Id, Platforms.Name,
	Developers.Id, Developers.Name,
	Publishers.Id, Publishers.Name
FROM Localizations
left join Games 
	on Localizations.Id=Games.LocalizationId
left join GamesPlatforms
	on GamesPlatforms.GameId=Games.Id
left join Platforms
	on Platforms.Id=GamesPlatforms.PlatformId
left join GamesDevelopers
	on GamesDevelopers.GameId=Games.Id
left join Developers
	on GamesDevelopers.DeveloperId=Developers.Id
left join Publishers
	on Games.PublisherId=Publishers.Id
WHERE Localizations.Id in 
(SELECT Localizations.id 
    FROM Localizations
    ORDER BY Id ASC
    OFFSET @offset ROWS
    FETCH NEXT @limit ROWS ONLY);",
    (localization, game, platform, developer, publisher) =>
    {
        if (game is not null)
        {
            if (platform is not null)
                game.Platforms.Add(platform);
            if (developer is not null)
                game.Developers.Add(developer);
            if (publisher is not null)
                game = game with { Publisher = publisher };

            localization.Games.Add(game);
        }
        return localization;
    }, new { offset, limit });

            var localizationsResult = localizations
                .GroupBy(d => d.Id)
                .Select(g =>
                {
                    Localization groupedLocalization = g.First() with
                    {
                        Games = g.SelectMany(d => d.Games)
                        .GroupBy(g => g.Id)
                        .Select(gameGroup =>
                        {
                            var game = gameGroup.First();
                            game = game with
                            {
                                Platforms = gameGroup.SelectMany(g => g.Platforms)
                            .DistinctBy(p => p.Id).ToList(),
                                Developers = gameGroup.SelectMany(g => g.Developers)
                                .DistinctBy(p => p.Id).ToList()
                            };
                            return game;
                        }).ToList()
                    };

                    return groupedLocalization;
                });

            return localizations;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
            await connection.ExecuteAsync(@"DELETE FROM 
Localizations WHERE Id=@id", new { id });
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
