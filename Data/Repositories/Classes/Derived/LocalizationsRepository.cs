using Dapper;
using Data.Repositories.Interfaces;
using Domain;
using Npgsql;

namespace Data.Repositories.Classes.Derived;

public sealed class LocalizationsRepository : Repository, IRepository<Localization>
{
    public LocalizationsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Localization entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Localizations
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

    public async Task AddRangeAsync(IEnumerable<Localization> localizations)
    {
        foreach (var localization in localizations)
            await AddAsync(localization);
    }

    public async Task<IEnumerable<Localization>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var localizations = await connection.QueryAsync<Localization>(@"SELECT Id, Name, Href 
FROM 
Localizations;");

            return localizations;
        }
    }

    public async Task<Localization> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var localization = await connection.QueryFirstOrDefaultAsync<Localization>(@"SELECT Id, Name, Href 
FROM 
Localizations
WHERE Id=@id;", new { id });

            return localization;
        }
    }

    public async Task<IEnumerable<Localization>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var localizations = await connection.QueryAsync<Localization>(@"SELECT Id, Name, Href 
FROM 
Localizations 
OFFSET @offset
LIMIT @limit;", new { offset, limit });

            return localizations;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM 
Localizations WHERE Id=@id", new { id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<Localization> UpdateAsync(Localization entity, long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"UPDATE Localizations set Name=@Name, Href=@Href 
where Id=@Id", new
            {
                entity.Name,
                entity.Href,
                id
            });
        }

        var localization = await GetAsync(id);
        return localization;
    }
}
