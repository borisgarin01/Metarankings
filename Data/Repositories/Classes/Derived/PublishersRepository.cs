using Dapper;
using Data.Repositories.Interfaces;
using Domain;
using Npgsql;

namespace Data.Repositories.Classes.Derived;
public sealed class PublishersRepository : Repository, IRepository<Publisher>
{
    public PublishersRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Publisher publisher)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Publishers
(Name, Url)
VALUES (@Name, @Url)
RETURNING Id;"
 , new
 {
     publisher.Name,
     publisher.Url
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
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var publishers = await connection.QueryAsync<Publisher>(@"SELECT Id, Name, Url 
FROM 
Publishers;");
            return publishers;
        }
    }

    public async Task<Publisher> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var publisher = await connection.QueryFirstOrDefaultAsync<Publisher>(@"SELECT Id, Name, Url 
FROM 
Publishers 
WHERE Id=@id", new { id });

            return publisher;
        }
    }

    public async Task<IEnumerable<Publisher>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var publishers = await connection.QueryAsync<Publisher>(@"SELECT Id, Name, Url 
FROM 
Publishers 
OFFSET @offset
LIMIT @limit;", new { offset, limit });

            return publishers;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
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
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var updatedPublisher = await connection.QueryFirstOrDefaultAsync<Publisher>(@"UPDATE Publishers set Name=@Name, Url=@Url 
where Id=@id 
returning Name, Url, Id", new
            {
                publisher.Name,
                publisher.Url,
                id
            });

            return updatedPublisher;
        }
    }
}
