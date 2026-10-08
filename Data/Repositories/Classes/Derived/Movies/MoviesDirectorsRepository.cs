using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;

namespace Data.Repositories.Classes.Derived.Movies;
public sealed class MoviesDirectorsRepository : Repository<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel>
{
    public MoviesDirectorsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMovieDirectorModel entity, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var insertedId = await connection.QueryFirstAsync<long>(new CommandDefinition(@"INSERT INTO MoviesDirectors (Name)
VALUES (@Name)
RETURNING Id;", new { entity.Name }, cancellationToken: cancellationToken));

            return insertedId;
        }
    }

    public override async Task<IEnumerable<MovieDirector>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var moviesDirectors = await connection.QueryAsync<MovieDirector>(new CommandDefinition(@"SELECT Id, Name 
FROM MoviesDirectors;", cancellationToken: cancellationToken));

            return moviesDirectors;
        }
    }

    public override async Task<MovieDirector> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var movieDirector = await connection.QueryFirstOrDefaultAsync<MovieDirector>(new CommandDefinition(@"SELECT Id, Name
FROM MoviesDirectors
WHERE Id=@id", new { id }, cancellationToken: cancellationToken));

            return movieDirector;
        }
    }

    public override async Task<IEnumerable<MovieDirector>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var moviesDirectors = await connection.QueryAsync<MovieDirector>(new CommandDefinition(@"SELECT Id, Name 
FROM MoviesDirectors
OFFSET @offset 
LIMIT @limit;", new { offset, limit }, cancellationToken: cancellationToken));

            return moviesDirectors;
        }
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
            await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM MoviesDirectors 
WHERE Id=@id", new { id }, cancellationToken: cancellationToken));
    }

    public override async Task<MovieDirector> UpdateAsync(UpdateMovieDirectorModel entity, long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var updatedMovieDirector = await connection.QueryFirstOrDefaultAsync<MovieDirector>(new CommandDefinition(@"UPDATE MoviesDirectors 
SET Name=@Name
WHERE Id=@id", new { entity.Name, id }, cancellationToken: cancellationToken));

            return updatedMovieDirector;
        }
    }
}
