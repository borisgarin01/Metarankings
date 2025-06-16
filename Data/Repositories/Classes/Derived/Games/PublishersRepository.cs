using Dapper;
using Data.Repositories.Interfaces;
using Domain.Games;
using Microsoft.Data.SqlClient;

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
VALUES (@Name)
RETURNING Id;"
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
            var publishers = await connection.QueryAsync<Publisher>(@"select 
publishers.id, publishers.name 
from publishers");

            if (publishers is null)
                return null;

            if (!publishers.Any())
                return publishers;

            foreach (var publisher in publishers)
            {
                var publisherGames = await connection.QueryAsync<Game>(@"SELECT Id, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer
FROM Games 
WHERE PublisherId=@PublisherId", new { PublisherId = publisher.Id });

                publisher.Games = publisherGames;

            }

            return publishers;
        }
    }

    public async Task<Publisher> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var publisher = await connection.QueryFirstOrDefaultAsync<Publisher>(@"select 
publishers.id, publishers.name
from publishers 
where Id=@id", new { id });

            if (publisher is null)
                return null;

            var publisherGames = await connection.QueryAsync<Game>(@"SELECT Id, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer
FROM Games 
WHERE PublisherId=@PublisherId", new { PublisherId = publisher.Id });

            publisher.Games = publisherGames;

            return publisher;
        }
    }

    public async Task<IEnumerable<Publisher>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var publishers = await connection.QueryAsync<Publisher>(@"select 
publishers.id, publishers.name
from publishers
OFFSET @offset 
limit @limit", new { offset, limit });

            if (publishers is null)
                return null;

            if (!publishers.Any())
                return publishers;

            foreach (var publisher in publishers)
            {
                var publisherGames = await connection.QueryAsync<Game>(@"SELECT Id, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer
FROM Games 
WHERE PublisherId=@PublisherId", new { PublisherId = publisher.Id });

                publisher.Games = publisherGames;

            }

            return publishers;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM 
Publishers WHERE Id=@id", new { id });
        }
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
where Id=@id 
returning Name, Id", new
            {
                publisher.Name,
                id
            });

            return updatedPublisher;
        }
    }
}
