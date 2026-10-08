using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesPersons;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesPersonsRepository : Repository<MoviePerson, AddMoviePersonModel, UpdateMoviePersonModel>
{
    public MoviesPersonsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMoviePersonModel entity)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstAsync<long>(@"INSERT INTO MoviesPersons (Name)
VALUES (@Name)
RETURNING Id;", new { entity.Name });
    }

    public override async Task<IEnumerable<MoviePerson>> GetAllAsync()
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<MoviePerson>(@"SELECT Id, Name
FROM MoviesPersons
ORDER BY Name;");
    }

    public override async Task<MoviePerson> GetAsync(long id)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<MoviePerson>(@"SELECT Id, Name
FROM MoviesPersons
WHERE Id=@id;", new { id });
    }

    public override async Task<IEnumerable<MoviePerson>> GetAsync(long offset, long limit)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<MoviePerson>(@"SELECT Id, Name
FROM MoviesPersons
ORDER BY Name
OFFSET @offset
LIMIT @limit;", new { offset, limit });
    }

    public override async Task RemoveAsync(long id)
    {
        using NpgsqlConnection connection = CreateConnection();
        await connection.ExecuteAsync(@"DELETE FROM MoviesPersons
WHERE Id=@id;", new { id });
    }

    public override async Task<MoviePerson> UpdateAsync(UpdateMoviePersonModel entity, long id)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<MoviePerson>(@"UPDATE MoviesPersons
SET Name=@Name
WHERE Id=@id
RETURNING Id, Name;", new { entity.Name, id });
    }
}
