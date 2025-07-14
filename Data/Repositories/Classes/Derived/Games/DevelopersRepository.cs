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
            IEnumerable<Developer> developers = await connection.QueryAsync<Developer, Game, Platform, Developer>(@"
            select 
                developers.id, developers.name, 
                games.Id, games.Name, games.Image, 
                games.publisherId, games.releasedate, 
                games.description, games.trailer,
                platforms.id, platforms.name
            from developers
            left join gamesdevelopers
                on gamesdevelopers.developerid=developers.id
            left join games
                on games.id=gamesdevelopers.gameid
            left join gamesplatforms
                on gamesplatforms.gameid=games.id
            left join platforms 
                on platforms.id=gamesplatforms.platformid",
                 (developerEntry, game, platform) =>
                 {
                     game.Platforms.Add(platform);
                     developerEntry.Games.Add(game);
                     return developerEntry;
                 });

            var developersResult = developers
   .GroupBy(d => d.Id)
   .Select(g =>
   {
       var groupedDeveloper = g.First();

       // Process games and platforms
       groupedDeveloper.Games = g
           .SelectMany(d => d.Games) // Flatten all games from all developer records
           .GroupBy(game => game.Id)  // Group games by ID
           .Select(gameGroup =>
           {
               var groupedGame = gameGroup.First();

               // Process platforms for this game
               groupedGame.Platforms = gameGroup
                   .SelectMany(g => g.Platforms) // Flatten all platforms
                   .GroupBy(p => p.Id)           // Group platforms by ID
                   .Select(p => p.First())        // Take first platform from each group
                   .ToList();

               return groupedGame;
           })
           .ToList();

       return groupedDeveloper;
   })
   .ToList();

            return developersResult;
        }
    }

    public async Task<Developer> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            IEnumerable<Developer> developers = await connection.QueryAsync<Developer, Game, Platform, Developer>(@"
            select 
                developers.id, developers.name, 
                games.Id, games.Name, games.Image, 
                games.publisherId, games.releasedate, 
                games.description, games.trailer,
                platforms.id, platforms.name
            from developers
            left join gamesdevelopers
                on gamesdevelopers.developerid=developers.id
            left join games
                on games.id=gamesdevelopers.gameid
            left join gamesplatforms
                on gamesplatforms.gameid=games.id
            left join platforms 
                on platforms.id=gamesplatforms.platformid
            WHERE developers.id=@Id",
                 (developerEntry, game, platform) =>
                 {
                     game.Platforms.Add(platform);
                     developerEntry.Games.Add(game);
                     return developerEntry;
                 }, new { id });

            var developersResult = developers
   .GroupBy(d => d.Id)
   .Select(g =>
   {
       var groupedDeveloper = g.First();

       // Process games and platforms
       groupedDeveloper.Games = g
           .SelectMany(d => d.Games) // Flatten all games from all developer records
           .GroupBy(game => game.Id)  // Group games by ID
           .Select(gameGroup =>
           {
               var groupedGame = gameGroup.First();

               // Process platforms for this game
               groupedGame.Platforms = gameGroup
                   .SelectMany(g => g.Platforms) // Flatten all platforms
                   .GroupBy(p => p.Id)           // Group platforms by ID
                   .Select(p => p.First())        // Take first platform from each group
                   .ToList();

               return groupedGame;
           })
           .ToList();

       return groupedDeveloper;
   })
   .ToList();

            return developersResult.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Developer>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            IEnumerable<Developer> developers = await connection.QueryAsync<Developer, Game, Platform, Developer>(@"
            select 
                devs.id, devs.name, 
                games.Id, games.Name, games.Image, 
                games.publisherId, games.releasedate, 
                games.description, games.trailer,
                platforms.id, platforms.name
            FROM (
                SELECT Id, Name 
                FROM Developers 
                ORDER BY Id
                OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY
            ) devs
            left join gamesdevelopers
                on gamesdevelopers.developerid=devs.id
            left join games
                on games.id=gamesdevelopers.gameid
            left join gamesplatforms
                on gamesplatforms.gameid=games.id
            left join platforms 
                on platforms.id=gamesplatforms.platformid",
                 (developerEntry, game, platform) =>
                 {
                     game.Platforms.Add(platform);
                     developerEntry.Games.Add(game);
                     return developerEntry;
                 }, new { offset, limit });

            var developersResult = developers
   .GroupBy(d => d.Id)
   .Select(g =>
   {
       var groupedDeveloper = g.First();

       // Process games and platforms
       groupedDeveloper.Games = g
           .SelectMany(d => d.Games) // Flatten all games from all developer records
           .GroupBy(game => game.Id)  // Group games by ID
           .Select(gameGroup =>
           {
               var groupedGame = gameGroup.First();

               // Process platforms for this game
               groupedGame.Platforms = gameGroup
                   .SelectMany(g => g.Platforms) // Flatten all platforms
                   .GroupBy(p => p.Id)           // Group platforms by ID
                   .Select(p => p.First())        // Take first platform from each group
                   .ToList();

               return groupedGame;
           })
           .ToList();

       return groupedDeveloper;
   })
   .ToList();

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
