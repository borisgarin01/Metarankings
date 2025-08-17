using Dapper;
using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Publishers;

namespace Data.Repositories.Classes.Derived.Games;
public sealed class PublishersRepository : Repository, IRepository<Publisher, AddPublisherModel, UpdatePublisherModel>
{
    public PublishersRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddPublisherModel publisher)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Publishers
(Name)
output inserted.id
VALUES (@Name);"
 , new
 {
     publisher.Name
 });
            return id;
        }
    }

    public async Task AddRangeAsync(IEnumerable<AddPublisherModel> publishers)
    {
        foreach (var publisher in publishers)
        {
            await AddAsync(publisher);
        }
    }

    public async Task<IEnumerable<Publisher>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var publisherDictionary = new Dictionary<long, Publisher>();

            await connection.QueryAsync<Publisher, Game, Publisher>(
                @"SELECT 
                p.Id, p.Name,
                g.Id, g.Name, g.Image, g.LocalizationId, g.PublisherId,
                g.ReleaseDate, g.Description, g.Trailer
              FROM Publishers p
              LEFT JOIN Games g ON g.PublisherId = p.Id",
                (publisher, game) =>
                {
                    if (!publisherDictionary.TryGetValue(publisher.Id, out var publisherEntry))
                    {
                        publisherEntry = publisher;
                        publisherEntry.Games = new List<Game>();
                        publisherDictionary.Add(publisherEntry.Id, publisherEntry);
                    }

                    if (game != null)
                    {
                        publisherEntry.Games.Add(game);
                    }

                    return publisherEntry;
                },
                splitOn: "Id"  // Split point between Publisher and Game columns
            );

            return publisherDictionary.Values;
        }
    }

    public async Task<Publisher> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var publisherDictionary = new Dictionary<long, Publisher>();

            await connection.QueryAsync<Publisher, Game, Publisher>(
                @"SELECT 
                p.Id, p.Name,
                g.Id, g.Name, g.Image, g.LocalizationId, g.PublisherId,
                g.ReleaseDate, g.Description, g.Trailer
              FROM Publishers p
              LEFT JOIN Games g ON g.PublisherId = p.Id
              WHERE p.Id = @id",
                (publisher, game) =>
                {
                    if (!publisherDictionary.TryGetValue(publisher.Id, out var publisherEntry))
                    {
                        publisherEntry = publisher;
                        publisherEntry.Games = new List<Game>();
                        publisherDictionary.Add(publisherEntry.Id, publisherEntry);
                    }

                    if (game != null)
                    {
                        publisherEntry.Games.Add(game);
                    }

                    return publisherEntry;
                },
                new { id },  // Correct parameter passing
                splitOn: "Id"  // Split point between Publisher and Game columns
            );

            return publisherDictionary.Values.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Publisher>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var publisherDictionary = new Dictionary<long, Publisher>();

            await connection.QueryAsync<Publisher, Game, Publisher>(@"
            SELECT 
                p.Id, p.Name,
                g.Id, g.Name, g.Image, g.LocalizationId, g.PublisherId,
                g.ReleaseDate, g.Description, g.Trailer
            FROM (
                SELECT Id, Name 
                FROM Publishers 
                ORDER BY Id
                OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY
            ) p
            LEFT JOIN Games g ON g.PublisherId = p.Id",
                (publisher, game) =>
                {
                    if (!publisherDictionary.TryGetValue(publisher.Id, out var publisherEntry))
                    {
                        publisherEntry = publisher;
                        publisherEntry.Games = new List<Game>();
                        publisherDictionary.Add(publisherEntry.Id, publisherEntry);
                    }

                    if (game != null)
                    {
                        publisherEntry.Games.Add(game);
                    }

                    return publisherEntry;
                },
                new { offset, limit },
                splitOn: "Id"
            );

            return publisherDictionary.Values;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM 
Publishers WHERE Id=@id", new { id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<Publisher> UpdateAsync(UpdatePublisherModel publisher, long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var updatedPublisher = await connection.QueryFirstOrDefaultAsync<Publisher>(@"UPDATE Publishers set Name=@Name 
output inserted.name, inserted.id
where Id=@id", new
            {
                publisher.Name,
                id
            });

            return updatedPublisher;
        }
    }
}
