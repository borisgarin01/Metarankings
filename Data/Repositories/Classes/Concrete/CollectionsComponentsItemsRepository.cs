using Dapper;
using Data.Repositories.Interfaces;
using Domain;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories.Classes.Concrete;
public sealed class CollectionsComponentsItemsRepository : RepositoryBase<CollectionsComponentItem>, ICollectionsComponentsItemsRepository
{
    public CollectionsComponentsItemsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task AddAsync(CollectionsComponentItem collectionsComponentItem)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"INSERT INTO CollectionsComponentItems
(ItemHref, Title, ImageSrc, ImageAlt, CategoryTitle, CategoryHref) 
VALUES(@ItemHref, @Title, @ImageSrc, @ImageAlt, @CategoryTitle, @CategoryHref);",
new
{
    collectionsComponentItem.ItemHref,
    collectionsComponentItem.Title,
    collectionsComponentItem.ImageSrc,
    collectionsComponentItem.ImageAlt,
    collectionsComponentItem.CategoryTitle,
    collectionsComponentItem.CategoryHref
});
        }
    }

    public async Task<IEnumerable<CollectionsComponentItem>> GetAllAsync()
    {
        IEnumerable<CollectionsComponentItem> collectionsComponentItems;

        using (var connection = new SqlConnection(ConnectionString))
        {
            collectionsComponentItems = await connection.QueryAsync<CollectionsComponentItem>("SELECT Id, ItemHref, Title, ImageSrc, ImageAlt, CategoryTitle, CategoryHref from CollectionsComponentItems");
        }

        return collectionsComponentItems;
    }
}
