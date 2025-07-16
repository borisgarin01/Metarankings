using Data.Repositories.Interfaces.Derived;
using Domain.Games;

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
            var localizationsDictionary = new Dictionary<long, Localization>();
            var gamesDictionary = new Dictionary<long, Game>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var developersDictionary = new Dictionary<long, Developer>();
            var publishersDictionary = new Dictionary<long, Publisher>();

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
        if (!localizationsDictionary.TryGetValue(localization.Id, out var localizationEntry))
        {
            localizationEntry = localization;
            localizationsDictionary.Add(localization.Id, localization);
        }

        if (game is not null && !gamesDictionary.TryGetValue(game.Id, out var gam))
            gamesDictionary.Add(game.Id, game);

        if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var platf))
            platformsDictionary.Add(platform.Id, platform);

        if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
            developersDictionary.Add(developer.Id, developer);

        game = game with
        {
            Platforms = platformsDictionary.Values.ToList(),
            Developers = developersDictionary.Values.ToList(),
            Publisher = publisher is not null && game.Publisher is null ? publisher : null,
            Localization = localization
        };

        localization = localization with { Games = gamesDictionary.Values };

        return localization;
    });

            return localizationsDictionary.Values.ToList();
        }
    }

    public async Task<Localization> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localizationsDictionary = new Dictionary<long, Localization>();
            var gamesDictionary = new Dictionary<long, Game>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var developersDictionary = new Dictionary<long, Developer>();
            var publishersDictionary = new Dictionary<long, Publisher>();

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
WHERE Localizations.Id=@Id;",
    (localization, game, platform, developer, publisher) =>
    {
        if (!localizationsDictionary.TryGetValue(localization.Id, out var localizationEntry))
        {
            localizationEntry = localization;
            localizationsDictionary.Add(localization.Id, localization);
        }

        if (game is not null && !gamesDictionary.TryGetValue(game.Id, out var gam))
            gamesDictionary.Add(game.Id, game);

        if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var platf))
            platformsDictionary.Add(platform.Id, platform);

        if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
            developersDictionary.Add(developer.Id, developer);

        game = game with
        {
            Platforms = platformsDictionary.Values.ToList(),
            Developers = developersDictionary.Values.ToList(),
            Publisher = publisher is not null && game.Publisher is null ? publisher : null,
            Localization = localization
        };

        localization = localization with { Games = gamesDictionary.Values };

        return localization;
    }, new { id });

            return localizationsDictionary.Values.FirstOrDefault();
        }
    }

    public async Task<Localization> GetByPlatformAsync(long id, long platformId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localizationsDictionary = new Dictionary<long, Localization>();
            var gamesDictionary = new Dictionary<long, Game>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var developersDictionary = new Dictionary<long, Developer>();
            var publishersDictionary = new Dictionary<long, Publisher>();

            var localization = await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(@"
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
    AND Platforms.Id=@PlatformId", (localization, game, platform, developer, publisher) =>
            {
                if (!localizationsDictionary.TryGetValue(localization.Id, out var localizationEntry))
                {
                    localizationEntry = localization;
                    localizationsDictionary.Add(localization.Id, localization);
                }

                if (game is not null && !gamesDictionary.TryGetValue(game.Id, out var gam))
                    gamesDictionary.Add(game.Id, game);

                if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var platf))
                    platformsDictionary.Add(platform.Id, platform);

                if (developer is not null && !developersDictionary.TryGetValue(developer.Id, out var dev))
                    developersDictionary.Add(developer.Id, developer);

                game = game with
                {
                    Platforms = platformsDictionary.Values.ToList(),
                    Developers = developersDictionary.Values.ToList(),
                    Publisher = publisher is not null && game.Publisher is null ? publisher : null,
                    Localization = localization
                };

                localization = localization with { Games = gamesDictionary.Values };

                return localization;
            }, new { id });

            return localizationsDictionary.Values.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Localization>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var localizationsDictionary = new Dictionary<long, Localization>();
            var gamesDictionary = new Dictionary<long, Game>();
            var platformsDictionary = new Dictionary<long, Platform>();
            var developersDictionary = new Dictionary<long, Developer>();

            var localization = await connection.QueryAsync<Localization, Game, Platform, Developer, Publisher, Localization>(@"
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
    FETCH NEXT @limit ROWS ONLY);", (localization, game, platform, developer, publisher) =>
            {
                if (!localizationsDictionary.TryGetValue(localization.Id, out var localizationEntry))
                {
                    localizationEntry = localization;
                    localizationEntry.Games = new List<Game>();
                    localizationsDictionary.Add(localization.Id, localization);
                }

                if (game is not null && !gamesDictionary.TryGetValue(game.Id, out var g))
                    gamesDictionary.Add(game.Id, game);

                if (platform is not null && !platformsDictionary.TryGetValue(platform.Id, out var platf))
                    platformsDictionary.Add(platform.Id, platform);

                if (developer is not null && !game.Developers.Any(d => d.Id == developer.Id))
                    developersDictionary.Add(developer.Id, developer);

                game = game with
                {
                    Developers = developersDictionary.Values.ToList(),
                    Platforms = platformsDictionary.Values.ToList(),
                    Localization = localization
                };

                return localization;
            },
    new
    {
        offset,
        limit
    });

            return localizationsDictionary.Values;
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
