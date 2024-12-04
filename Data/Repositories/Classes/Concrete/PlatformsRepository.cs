using Dapper;
using Data.Repositories.Interfaces;
using Domain;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories.Classes.Concrete;
public sealed class GamesPlatformsRepository : RepositoryBase<GamePlatform>, IGamesPlatformsRepository
{
    public GamesPlatformsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task AddAsync(GamePlatform platform)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync("INSERT INTO Platforms (Name, Href) values (@Name, @Href);", new { platform.Name, platform.Href });
        }
    }

    public async Task<IEnumerable<GamePlatform>> GetAllAsync()
    {
        IEnumerable<GamePlatform> platforms;

        using (var connection = new SqlConnection(ConnectionString))
        {
            platforms = await connection.QueryAsync<GamePlatform>("SELECT Id, Name, Href from Platforms");
        }

        return platforms;
    }
}
