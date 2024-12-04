using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories.Interfaces;
public interface ICollectionsComponentsItemsRepository
{
    public Task AddAsync(CollectionsComponentItem collectionsComponentItem);
    public Task<IEnumerable<CollectionsComponentItem>> GetAllAsync();
}
