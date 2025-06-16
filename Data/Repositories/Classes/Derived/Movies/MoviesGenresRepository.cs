using Dapper;
using Data.Repositories.Interfaces;
using Domain.Movies;
using Microsoft.Data.SqlClient;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesGenresRepository : Repository, IRepository<MovieGenre>
{
    public MoviesGenresRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(MovieGenre entity)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var insertedId = await connection.QueryFirstOrDefaultAsync<long>(@"INSERT INTO MoviesGenres(Name) 
VALUES
(@Name) 
RETURNING 
Id;", new { entity.Name });

            return insertedId;
        }
    }

    public async Task AddRangeAsync(IEnumerable<MovieGenre> entities)
    {
        foreach (var movieGenre in entities)
        {
            await AddAsync(movieGenre);
        }
    }

    public async Task<IEnumerable<MovieGenre>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var moviesGenres = await connection.QueryAsync<MovieGenre>(@"SELECT Id, Name 
FROM MoviesGenres;");

            return moviesGenres;
        }
    }

    public async Task<MovieGenre> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var moviesGenres = await connection.QueryFirstOrDefaultAsync<MovieGenre>(@"SELECT Id, Name 
FROM MoviesGenres
WHERE Id=@id;", new { id });

            return moviesGenres;
        }
    }

    public async Task<IEnumerable<MovieGenre>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var moviesGenres = await connection.QueryAsync<MovieGenre>(@"SELECT Id, Name 
FROM MoviesGenres
OFFSET @offset
LIMIT @limit;", new { offset, limit });

            return moviesGenres;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM MoviesGenres WHERE Id=@id", new { id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<MovieGenre> UpdateAsync(MovieGenre movieGenre, long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var updatedMovieGenre = await connection.QueryFirstOrDefaultAsync<MovieGenre>(@"UPDATE MoviesGenres set Name=@Name 
WHERE Id=@id 
returning Name, Id", new
            {
                movieGenre.Name,
                id
            });

            return updatedMovieGenre;
        }
    }
}
