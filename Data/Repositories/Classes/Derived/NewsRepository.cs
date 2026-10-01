using Data.Repositories.Interfaces;
using Domain.Common.News;

namespace Data.Repositories.Classes.Derived;

public sealed class NewsRepository
    : Repository, IRepository<NewsItem, AddNewsItemDbModel, UpdateNewsItemDbModel>
{
    public NewsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddNewsItemDbModel entity)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        var id = await connection.QueryFirstAsync<long>(@"INSERT INTO News
(Title, UserId, TextContent, ImageSource, PublishTimestamp)
VALUES (@Title, @UserId, @TextContent, @ImageSource, @PublishTimestamp)
RETURNING Id;",
            new
            {
                entity.Title,
                entity.UserId,
                entity.TextContent,
                entity.ImageSource,
                entity.PublishTimestamp
            });
        return id;
    }

    public async Task AddRangeAsync(IEnumerable<AddNewsItemDbModel> entities)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity);
        }
    }

    public async Task<IEnumerable<NewsItem>> GetAllAsync()
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        var news = await connection.QueryAsync<NewsItem>(@"
            SELECT Id, Title, UserId, TextContent, ImageSource, PublishTimestamp
            FROM News
            ORDER BY PublishTimestamp DESC");
        return news;
    }

    public async Task<NewsItem?> GetAsync(long id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        var newsItem = await connection.QueryFirstOrDefaultAsync<NewsItem>(@"
            SELECT Id, Title, UserId, TextContent, ImageSource, PublishTimestamp
            FROM News
            WHERE Id=@id",
            new { id });
        return newsItem;
    }

    public async Task<IEnumerable<NewsItem>> GetAsync(long offset, long limit)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        var news = await connection.QueryAsync<NewsItem>(@"
            SELECT Id, Title, UserId, TextContent, ImageSource, PublishTimestamp
            FROM News
            ORDER BY PublishTimestamp DESC
            OFFSET @Offset LIMIT @Limit",
            new { Offset = offset, Limit = limit });
        return news;
    }

    public async Task RemoveAsync(long id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(@"DELETE FROM News WHERE Id=@id", new { id });
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(
            @"DELETE FROM News WHERE Id = ANY(@ids)",
            new { ids = ids.ToArray() });
    }

    public async Task<NewsItem?> UpdateAsync(UpdateNewsItemDbModel entity, long id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        var updatedNews = await connection.QueryFirstOrDefaultAsync<NewsItem>(@"
            UPDATE News
            SET Title=@Title,
                UserId=@UserId,
                TextContent=@TextContent,
                ImageSource=@ImageSource
            WHERE Id=@id
            RETURNING Id, Title, UserId, TextContent, ImageSource, PublishTimestamp;",
            new
            {
                entity.Title,
                entity.UserId,
                entity.TextContent,
                entity.ImageSource,
                id
            });
        return updatedNews;
    }
}