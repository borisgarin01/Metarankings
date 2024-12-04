using Dapper;
using Data.Repositories.Interfaces;
using Domain;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories.Classes.Concrete;
public sealed class GenresRepository : RepositoryBase<Genre>, IGenresRepository
{
    public GenresRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task AddAsync(Genre genre)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync("INSERT INTO Genres (Name, Href) values (@Name, @Href);", new { genre.Name, genre.Href });
        }
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        IEnumerable<Genre> genres;

        using (var connection = new SqlConnection(ConnectionString))
        {
            genres = await connection.QueryAsync<Genre>("SELECT Id, Name, Href from Genres");
        }

        return genres;
    }
}
