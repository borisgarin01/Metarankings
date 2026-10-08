using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesCountries;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesCountriesRepository : Repository<MovieCountry, AddMovieCountryModel, UpdateMovieCountryModel>
{
    public MoviesCountriesRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMovieCountryModel entity)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstAsync<long>(@"INSERT INTO MoviesCountries (Name)
VALUES (@Name)
RETURNING Id;", new { entity.Name });
    }

    public override async Task<IEnumerable<MovieCountry>> GetAllAsync()
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<MovieCountry>(@"SELECT Id, Name
FROM MoviesCountries
ORDER BY Name;");
    }

    public override async Task<MovieCountry> GetAsync(long id)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<MovieCountry>(@"SELECT Id, Name
FROM MoviesCountries
WHERE Id=@id;", new { id });
    }

    public override async Task<IEnumerable<MovieCountry>> GetAsync(long offset, long limit)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryAsync<MovieCountry>(@"SELECT Id, Name
FROM MoviesCountries
ORDER BY Name
OFFSET @offset
LIMIT @limit;", new { offset, limit });
    }

    public override async Task RemoveAsync(long id)
    {
        using NpgsqlConnection connection = CreateConnection();
        await connection.ExecuteAsync(@"DELETE FROM MoviesCountries
WHERE Id=@id;", new { id });
    }

    public override async Task<MovieCountry> UpdateAsync(UpdateMovieCountryModel entity, long id)
    {
        using NpgsqlConnection connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<MovieCountry>(@"UPDATE MoviesCountries
SET Name=@Name
WHERE Id=@id
RETURNING Id, Name;", new { entity.Name, id });
    }
}
