using Dapper;
using Data.Repositories.Interfaces;
using Domain;
using Npgsql;

namespace Data.Repositories.Classes.Derived;

public sealed class PlatformsRepository : Repository, IRepository<Platform>
{
    public PlatformsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Platform entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Platforms
(Name, Href)
VALUES (@Name, @Href)
RETURNING Id;"
 , new
 {
     entity.Name,
     entity.Href
 });
            return id;
        }
    }

    public async Task AddRangeAsync(IEnumerable<Platform> entities)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity);
        }
    }

    public async Task<IEnumerable<Platform>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var platforms = await connection.QueryAsync<Platform>(@"SELECT Id, Name, Href 
FROM 
Platforms;");
            return platforms;
        }
    }

    public async Task<Platform> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var platform = await connection.QueryFirstOrDefaultAsync<Platform>(@"SELECT Id, Name, Href 
FROM 
Platforms 
WHERE Id=@id", new { id });

            return platform;
        }
    }

    public async Task<IEnumerable<Platform>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var platforms = await connection.QueryAsync<Platform>(@"SELECT Id, Name, Href 
FROM 
Platforms 
OFFSET @offset
LIMIT @limit;", new { offset, limit });

            return platforms;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM 
Platforms WHERE Id=@id", new { id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<Platform> UpdateAsync(Platform entity, long id)
    {
        var platform = await GetAsync(id);

        if (platform is not null)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.ExecuteAsync(@"UPDATE Platforms set Name=@Name, Href=@Href 
where Id=@Id", new
                {
                    entity.Name,
                    entity.Href,
                    platform.Id
                });
            }

            platform = await GetAsync(id);
            return platform;
        }

        return null;
    }
}
