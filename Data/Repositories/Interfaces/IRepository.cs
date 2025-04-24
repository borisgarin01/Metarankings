namespace Data.Repositories.Interfaces;
public interface IRepository<T>
{
    public Task<IEnumerable<T>> GetAllAsync();
    public Task<T> GetAsync(long id);
    public Task<IEnumerable<T>> GetAsync(long offset, long limit);
    public Task<long> AddAsync(T entity);
    public Task<T> UpdateAsync(T entity, long id);
    public Task AddRangeAsync(IEnumerable<T> entities);
    public Task RemoveAsync(long id);
    public Task RemoveRangeAsync(IEnumerable<long> ids);
}
