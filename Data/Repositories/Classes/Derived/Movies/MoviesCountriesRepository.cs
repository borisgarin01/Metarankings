using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesCountries;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesCountriesRepository : Repository<MovieCountry, AddMovieCountryModel, UpdateMovieCountryModel>
{
    public MoviesCountriesRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMovieCountryModel entity, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstAsync<long>(new CommandDefinition(@"INSERT INTO MoviesCountries (Name)
VALUES (@Name)
RETURNING Id;", new { entity.Name }, cancellationToken: cancellationToken));
    }

    public override async Task<IEnumerable<MovieCountry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<MovieCountry>(new CommandDefinition(@"SELECT Id, Name
FROM MoviesCountries
ORDER BY Name;", cancellationToken: cancellationToken));
    }

    public override async Task<MovieCountry> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<MovieCountry>(new CommandDefinition(@"SELECT Id, Name
FROM MoviesCountries
WHERE Id=@id;", new { id }, cancellationToken: cancellationToken));
    }

    public override async Task<IEnumerable<MovieCountry>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<MovieCountry>(new CommandDefinition(@"SELECT Id, Name
FROM MoviesCountries
ORDER BY Name
OFFSET @offset
LIMIT @limit;", new { offset, limit }, cancellationToken: cancellationToken));
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM MoviesCountries
WHERE Id=@id;", new { id }, cancellationToken: cancellationToken));
    }

    public override async Task<MovieCountry> UpdateAsync(UpdateMovieCountryModel entity, long id, CancellationToken cancellationToken = default)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<MovieCountry>(new CommandDefinition(@"UPDATE MoviesCountries
SET Name=@Name
WHERE Id=@id
RETURNING Id, Name;", new { entity.Name, id }, cancellationToken: cancellationToken));
    }
}
