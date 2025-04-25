using Dapper;
using Data.Repositories.Interfaces;
using Domain;
using Npgsql;

namespace Data.Repositories.Classes.Derived;
public sealed class DevelopersRepository : Repository, IRepository<Developer>
{
    public DevelopersRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Developer developer)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Developers
(Name, Url)
VALUES (@Name, @Url)
RETURNING Id;"
 , new
 {
     developer.Name,
     developer.Url
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
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var developers = await connection.QueryAsync<Developer>(@"SELECT Id, Name, Url 
FROM 
Developers;");
            return developers;
        }
    }

    public async Task<Developer> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var developer = await connection.QueryFirstOrDefaultAsync<Developer>(@"SELECT Id, Name, Url 
FROM 
Developers 
WHERE Id=@id", new { id });

            return developer;
        }
    }

    public async Task<IEnumerable<Developer>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var developers = await connection.QueryAsync<Developer>(@"SELECT Id, Name, Url 
FROM 
Developers 
OFFSET @offset
LIMIT @limit;", new { offset, limit });

            return developers;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
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
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var updatedDeveloper = await connection.QueryFirstOrDefaultAsync<Developer>(@"UPDATE Developers set Name=@Name, Url=@Url 
where Id=@id
returning Name, Url, Id", new
            {
                developer.Name,
                developer.Url,
                id
            });

            return updatedDeveloper;
        }
    }
}
