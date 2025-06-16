using Dapper;
using Data.Repositories.Interfaces;
using Domain.Movies;
using Microsoft.Data.SqlClient;

namespace Data.Repositories.Classes.Derived.Movies;
public sealed class MoviesStudiosRepository : Repository, IRepository<MovieStudio>
{
    public MoviesStudiosRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(MovieStudio entity)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var insertedId = await connection.QueryFirstOrDefaultAsync<long>(@"INSERT INTO MoviesStudios 
(Name) 
VALUES(@Name)
RETURNING Id", new { Name = entity.Name });

            return insertedId;
        }
    }

    public async Task AddRangeAsync(IEnumerable<MovieStudio> entities)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity);
        }
    }

    public async Task<IEnumerable<MovieStudio>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var moviesStudios = await connection.QueryAsync<MovieStudio>(@"
SELECT Id, Name 
FROM MoviesStudios;");

            return moviesStudios;
        }
    }

    public async Task<MovieStudio> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var movieStudio = await connection.QueryFirstOrDefaultAsync<MovieStudio>(@"SELECT Id, Name 
FROM MoviesStudios
WHERE Id=@id", new { id });

            return movieStudio;
        }
    }

    public async Task<IEnumerable<MovieStudio>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var moviesStudios = await connection.QueryAsync<MovieStudio>(@"
SELECT Id, Name 
FROM MoviesStudios
OFFSET @offset 
LIMIT @limit;", new { offset, limit });

            return moviesStudios;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM MoviesStudios
WHERE Id=@id", new { id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<MovieStudio> UpdateAsync(MovieStudio entity, long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var updatedMovieStudio = await connection.QueryFirstOrDefaultAsync<MovieStudio>(@"UPDATE MoviesStudios 
SET
Name=@Name
WHERE Id=@id
RETURNING Name, Id", new { entity.Name, id });

            return updatedMovieStudio;
        }
    }
}
