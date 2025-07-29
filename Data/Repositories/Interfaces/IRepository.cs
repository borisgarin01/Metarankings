namespace Data.Repositories.Interfaces;
public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetAsync(long id);
    Task<IEnumerable<T>> GetAsync(long offset, long limit);
    Task<long> AddAsync(T entity);
    Task<T> UpdateAsync(T entity, long id);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task RemoveAsync(long id);
    Task RemoveRangeAsync(IEnumerable<long> ids);
}
