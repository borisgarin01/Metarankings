using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesPersons;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesPersonsRepository : Repository<MoviePerson, AddMoviePersonModel, UpdateMoviePersonModel>
{
    public MoviesPersonsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMoviePersonModel entity, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstAsync<long>(new CommandDefinition(@"INSERT INTO MoviesPersons (Name)
VALUES (@Name)
RETURNING Id;", new { entity.Name }, cancellationToken: cancellationToken));
    }

    public override async Task<IEnumerable<MoviePerson>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<MoviePerson>(new CommandDefinition(@"SELECT Id, Name
FROM MoviesPersons
ORDER BY Name;", cancellationToken: cancellationToken));
    }

    public override async Task<MoviePerson> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<MoviePerson>(new CommandDefinition(@"SELECT Id, Name
FROM MoviesPersons
WHERE Id=@id;", new { id }, cancellationToken: cancellationToken));
    }

    public override async Task<IEnumerable<MoviePerson>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<MoviePerson>(new CommandDefinition(@"SELECT Id, Name
FROM MoviesPersons
ORDER BY Name
OFFSET @offset
LIMIT @limit;", new { offset, limit }, cancellationToken: cancellationToken));
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM MoviesPersons
WHERE Id=@id;", new { id }, cancellationToken: cancellationToken));
    }

    public override async Task<MoviePerson> UpdateAsync(UpdateMoviePersonModel entity, long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<MoviePerson>(new CommandDefinition(@"UPDATE MoviesPersons
SET Name=@Name
WHERE Id=@id
RETURNING Id, Name;", new { entity.Name, id }, cancellationToken: cancellationToken));
    }
}
