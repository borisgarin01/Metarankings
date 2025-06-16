using Dapper;
using Data.Repositories.Interfaces;
using Domain.Movies;
using Npgsql;

namespace Data.Repositories.Classes.Derived.Movies;
public sealed class MoviesDirectorsRepository : Repository, IRepository<MovieDirector>
{
    public MoviesDirectorsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(MovieDirector entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var insertedId = await connection.QueryFirstAsync<long>(@"INSERT INTO MoviesDirectors (Name)
VALUES (@Name)
RETURNING Id", new { entity.Name });

            return insertedId;
        }
    }

    public async Task AddRangeAsync(IEnumerable<MovieDirector> entities)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity);
        }
    }

    public async Task<IEnumerable<MovieDirector>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var moviesDirectors = await connection.QueryAsync<MovieDirector>(@"SELECT Id, Name 
FROM MoviesDirectors;");

            return moviesDirectors;
        }
    }

    public async Task<MovieDirector> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var movieDirector = await connection.QueryFirstOrDefaultAsync<MovieDirector>(@"SELECT Id, Name
FROM MoviesDirectors
WHERE Id=@id", new { id });

            return movieDirector;
        }
    }

    public async Task<IEnumerable<MovieDirector>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var moviesDirectors = await connection.QueryAsync<MovieDirector>(@"SELECT Id, Name 
FROM MoviesDirectors
OFFSET @offset 
LIMIT @limit;", new { offset, limit });

            return moviesDirectors;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM MoviesDirectors 
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

    public async Task<MovieDirector> UpdateAsync(MovieDirector entity, long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var updatedMovieDirector = await connection.QueryFirstOrDefaultAsync<MovieDirector>(@"UPDATE MoviesDirectors 
SET Name=@Name
WHERE Id=@id",
new { entity.Name, id });

            return updatedMovieDirector;
        }
    }
}
