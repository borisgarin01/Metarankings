using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesStudios;

namespace Data.Repositories.Classes.Derived.Movies;
public sealed class MoviesStudiosRepository : Repository<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>
{
    public MoviesStudiosRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMovieStudioModel entity, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var insertedId = await connection.QueryFirstOrDefaultAsync<long>(new CommandDefinition(@"INSERT INTO MoviesStudios 
(Name) 
VALUES(@Name)
RETURNING Id;", new { Name = entity.Name }, cancellationToken: cancellationToken));

            return insertedId;
        }
    }

    public override async Task<IEnumerable<MovieStudio>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var moviesStudios = await connection.QueryAsync<MovieStudio>(new CommandDefinition(@"
SELECT Id, Name 
FROM MoviesStudios;", cancellationToken: cancellationToken));

            return moviesStudios;
        }
    }

    public override async Task<MovieStudio> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var movieStudio = await connection.QueryFirstOrDefaultAsync<MovieStudio>(new CommandDefinition(@"SELECT Id, Name 
FROM MoviesStudios
WHERE Id=@id", new { id }, cancellationToken: cancellationToken));

            return movieStudio;
        }
    }

    public override async Task<IEnumerable<MovieStudio>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var moviesStudios = await connection.QueryAsync<MovieStudio>(new CommandDefinition(@"
SELECT Id, Name 
FROM MoviesStudios
OFFSET @offset 
LIMIT @limit;", new { offset, limit }, cancellationToken: cancellationToken));

            return moviesStudios;
        }
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
            await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM MoviesStudios
WHERE Id=@id", new { id }, cancellationToken: cancellationToken));
    }

    public override async Task<MovieStudio> UpdateAsync(UpdateMovieStudioModel entity, long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var updatedMovieStudio = await connection.QueryFirstOrDefaultAsync<MovieStudio>(new CommandDefinition(@"UPDATE MoviesStudios 
SET
Name=@Name
WHERE Id=@Id
RETURNING Id, Name", new { entity.Name, Id = id }, cancellationToken: cancellationToken));

            return updatedMovieStudio;
        }
    }
}
