using Dapper;
using Data.Repositories.Interfaces;
using Domain;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories.Classes.Concrete;
public sealed class MoviesGenresRepository : RepositoryBase<Genre>, IMoviesGenresRepository
{
    public MoviesGenresRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task AddAsync(Genre genre)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync("INSERT INTO MoviesGenres (Name, Href) values (@Name, @Href);", new { genre.Name, genre.Href });
        }
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        IEnumerable<Genre> MoviesGenres;

        using (var connection = new SqlConnection(ConnectionString))
        {
            MoviesGenres = await connection.QueryAsync<Genre>("SELECT Id, Name, Href from MoviesGenres");
        }

        return MoviesGenres;
    }
}
