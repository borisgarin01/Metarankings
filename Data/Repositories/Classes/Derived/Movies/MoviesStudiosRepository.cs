using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesStudios;

namespace Data.Repositories.Classes.Derived.Movies;
public sealed class MoviesStudiosRepository : Repository<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>
{
    public MoviesStudiosRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMovieStudioModel entity)
    {
        using (var connection = CreateConnection())
        {
            var insertedId = await connection.QueryFirstOrDefaultAsync<long>(@"INSERT INTO MoviesStudios 
(Name) 
VALUES(@Name)
RETURNING Id;", new { Name = entity.Name });

            return insertedId;
        }
    }

    public override async Task<IEnumerable<MovieStudio>> GetAllAsync()
    {
        using (var connection = CreateConnection())
        {
            var moviesStudios = await connection.QueryAsync<MovieStudio>(@"
SELECT Id, Name 
FROM MoviesStudios;");

            return moviesStudios;
        }
    }

    public override async Task<MovieStudio> GetAsync(long id)
    {
        using (var connection = CreateConnection())
        {
            var movieStudio = await connection.QueryFirstOrDefaultAsync<MovieStudio>(@"SELECT Id, Name 
FROM MoviesStudios
WHERE Id=@id", new { id });

            return movieStudio;
        }
    }

    public override async Task<IEnumerable<MovieStudio>> GetAsync(long offset, long limit)
    {
        using (var connection = CreateConnection())
        {
            var moviesStudios = await connection.QueryAsync<MovieStudio>(@"
SELECT Id, Name 
FROM MoviesStudios
OFFSET @offset 
LIMIT @limit;", new { offset, limit });

            return moviesStudios;
        }
    }

    public override async Task RemoveAsync(long id)
    {
        using (var connection = CreateConnection())
            await connection.ExecuteAsync(@"DELETE FROM MoviesStudios
WHERE Id=@id", new { id });
    }

    public override async Task<MovieStudio> UpdateAsync(UpdateMovieStudioModel entity, long id)
    {
        using (var connection = CreateConnection())
        {
            var updatedMovieStudio = await connection.QueryFirstOrDefaultAsync<MovieStudio>(@"UPDATE MoviesStudios 
SET
Name=@Name
WHERE Id=@Id
RETURNING Id, Name", new { entity.Name, Id = id });

            return updatedMovieStudio;
        }
    }
}
