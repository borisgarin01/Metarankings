using Data.Repositories.Interfaces;
using Domain.Games;

namespace Data.Repositories.Classes.Derived.Games;
public sealed class PublishersRepository : Repository, IRepository<Publisher>
{
    public PublishersRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Publisher publisher)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Publishers
(Name)
output inserted.id
VALUES (@Name);"
    , new
    {
        publisher.Name
    });
            return id;
        }
    }

    public async Task AddRangeAsync(IEnumerable<Publisher> publishers)
    {
        foreach (var publisher in publishers)
        {
            await AddAsync(publisher);
        }
    }

    public async Task<IEnumerable<Publisher>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var publishersDictionary = new Dictionary<long, Publisher>();

            var publisher = await connection.QueryAsync<Publisher, Game, Publisher>(@"select 
	publishers.Id, publishers.Name,
	games.Id, games.Name, games.Image, games.LocalizationId,
	games.PublisherId,games.ReleaseDate, games.Description,
	games.Trailer
	FROM Publishers 
		LEFT JOIN Games 
			ON Games.PublisherId=Publishers.Id", (publisher, game) =>
            {
                if (!publishersDictionary.TryGetValue(publisher.Id, out var publisherEntry))
                {
                    publisherEntry = publisher;
                    publisherEntry.Games = new List<Game>();
                    publishersDictionary.Add(publisher.Id, publisherEntry);
                }

                if (game is not null && !publisherEntry.Games.Any(d => d.Id == publisher.Id))
                    publisherEntry.Games.Add(game);

                return publisherEntry;
            });

            return publishersDictionary.Values;
        }
    }

    public async Task<Publisher> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var publishersDictionary = new Dictionary<long, Publisher>();

            var publisher = await connection.QueryAsync<Publisher, Game, Publisher>(@"select 
	publishers.Id, publishers.Name,
	games.Id, games.Name, games.Image, games.LocalizationId,
	games.PublisherId,games.ReleaseDate, games.Description,
	games.Trailer
	FROM Publishers 
		LEFT JOIN Games 
			ON Games.PublisherId=Publishers.Id
WHERE PublisherId=@PublisherId", (publisher, game) =>
            {
                if (!publishersDictionary.TryGetValue(publisher.Id, out var publisherEntry))
                {
                    publisherEntry = publisher;
                    publisherEntry.Games = new List<Game>();
                    publishersDictionary.Add(publisher.Id, publisherEntry);
                }

                if (game is not null && !publisherEntry.Games.Any(d => d.Id == publisher.Id))
                    publisherEntry.Games.Add(game);

                return publisherEntry;
            }, new { PublisherId = id });

            return publishersDictionary.Values.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Publisher>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var publishersDictionary = new Dictionary<long, Publisher>();

            var publisher = await connection.QueryAsync<Publisher, Game, Publisher>(@"select 
	publishers.Id, publishers.Name,
	games.Id, games.Name, games.Image, games.LocalizationId,
	games.PublisherId,games.ReleaseDate, games.Description,
	games.Trailer
	FROM Publishers 
		LEFT JOIN Games 
			ON Games.PublisherId=Publishers.Id
    WHERE Publishers.Id IN 
        IN(SELECT Id FROM Publishers ORDER BY Id ASC OFFSET @offset ROWS
            FETCH NEXT @limit ROWS ONLY)", (publisher, game) =>
            {
                if (!publishersDictionary.TryGetValue(publisher.Id, out var publisherEntry))
                {
                    publisherEntry = publisher;
                    publisherEntry.Games = new List<Game>();
                    publishersDictionary.Add(publisher.Id, publisherEntry);
                }

                if (game is not null && !publisherEntry.Games.Any(d => d.Id == publisher.Id))
                    publisherEntry.Games.Add(game);

                return publisherEntry;
            });

            return publishersDictionary.Values;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
            await connection.ExecuteAsync(@"DELETE FROM 
Publishers WHERE Id=@id", new { id });
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<Publisher> UpdateAsync(Publisher publisher, long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var updatedPublisher = await connection.QueryFirstOrDefaultAsync<Publisher>(@"UPDATE Publishers set Name=@Name 
output inserted.name, inserted.id
where Id=@id", new
            {
                publisher.Name,
                id
            });

            return updatedPublisher;
        }
    }
}
