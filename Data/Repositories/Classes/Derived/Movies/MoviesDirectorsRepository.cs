using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;

namespace Data.Repositories.Classes.Derived.Movies;
public sealed class MoviesDirectorsRepository : Repository<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel>
{
    public MoviesDirectorsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMovieDirectorModel entity)
    {
        using (var connection = CreateConnection())
        {
            var insertedId = await connection.QueryFirstAsync<long>(@"INSERT INTO MoviesDirectors (Name)
VALUES (@Name)
RETURNING Id;", new { entity.Name });

            return insertedId;
        }
    }

    public override async Task<IEnumerable<MovieDirector>> GetAllAsync()
    {
        using (var connection = CreateConnection())
        {
            var moviesDirectors = await connection.QueryAsync<MovieDirector>(@"SELECT Id, Name 
FROM MoviesDirectors;");

            return moviesDirectors;
        }
    }

    public override async Task<MovieDirector> GetAsync(long id)
    {
        using (var connection = CreateConnection())
        {
            var movieDirector = await connection.QueryFirstOrDefaultAsync<MovieDirector>(@"SELECT Id, Name
FROM MoviesDirectors
WHERE Id=@id", new { id });

            return movieDirector;
        }
    }

    public override async Task<IEnumerable<MovieDirector>> GetAsync(long offset, long limit)
    {
        using (var connection = CreateConnection())
        {
            var moviesDirectors = await connection.QueryAsync<MovieDirector>(@"SELECT Id, Name 
FROM MoviesDirectors
OFFSET @offset 
LIMIT @limit;", new { offset, limit });

            return moviesDirectors;
        }
    }

    public override async Task RemoveAsync(long id)
    {
        using (var connection = CreateConnection())
            await connection.ExecuteAsync(@"DELETE FROM MoviesDirectors 
WHERE Id=@id", new { id });
    }

    public override async Task<MovieDirector> UpdateAsync(UpdateMovieDirectorModel entity, long id)
    {
        using (var connection = CreateConnection())
        {
            var updatedMovieDirector = await connection.QueryFirstOrDefaultAsync<MovieDirector>(@"UPDATE MoviesDirectors 
SET Name=@Name
WHERE Id=@id",
    new { entity.Name, id });

            return updatedMovieDirector;
        }
    }
}
