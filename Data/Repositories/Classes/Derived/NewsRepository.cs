using Data.Repositories.Interfaces;
using Domain.Common.News;
using IdentityLibrary.DTOs;

namespace Data.Repositories.Classes.Derived;

public sealed class NewsRepository
    : Repository<NewsItem, AddNewsItemDbModel, UpdateNewsItemDbModel>
{
    public NewsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddNewsItemDbModel entity)
    {
        using var connection = CreateConnection();
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

    public override async Task<IEnumerable<NewsItem>> GetAllAsync()
    {
        using var connection = CreateConnection();
        var news = await connection.QueryAsync<NewsItem>(@"
            SELECT Id, Title, UserId, TextContent, ImageSource, PublishTimestamp
            FROM News
            ORDER BY PublishTimestamp DESC");
        return news;
    }

    public override async Task<NewsItem?> GetAsync(long id)
    {
        using var connection = CreateConnection();

        const string sql = @"
        SELECT  n.Id,
                n.Title,
                n.UserId,
                n.TextContent,
                n.ImageSource,
                n.PublishTimestamp,
                u.Id,
                u.UserName,
                u.NormalizedUserName,
                u.Email,
                u.NormalizedEmail,
                u.EmailConfirmed,
                u.PasswordHash,
                u.PhoneNumber,
                u.PhoneNumberConfirmed,
                u.TwoFactorEnabled
        FROM News n
        LEFT JOIN ApplicationUsers u ON u.Id = n.UserId
        WHERE n.Id = @id";

        var result = await connection.QueryAsync<NewsItem, ApplicationUser, NewsItem>(
            sql,
            (news, user) =>
            {
                news.ApplicationUser = user;
                return news;
            },
            new { id },
            splitOn: "Id");

        return result.FirstOrDefault();
    }

    public override async Task<IEnumerable<NewsItem>> GetAsync(long offset, long limit)
    {
        using var connection = CreateConnection();
        var news = await connection.QueryAsync<NewsItem>(@"
            SELECT Id, Title, UserId, TextContent, ImageSource, PublishTimestamp
            FROM News
            ORDER BY PublishTimestamp DESC
            OFFSET @Offset LIMIT @Limit",
            new { Offset = offset, Limit = limit });
        return news;
    }

    public override async Task RemoveAsync(long id)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(@"DELETE FROM News WHERE Id=@id", new { id });
    }

    public override async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(
            @"DELETE FROM News WHERE Id = ANY(@ids)",
            new { ids = ids.ToArray() });
    }

    public override async Task<NewsItem?> UpdateAsync(UpdateNewsItemDbModel entity, long id)
    {
        using var connection = CreateConnection();
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