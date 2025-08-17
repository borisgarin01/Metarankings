namespace Data.Repositories.Interfaces;
public interface IRepository<T, TAdd, TUpdate>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetAsync(long id);
    Task<IEnumerable<T>> GetAsync(long offset, long limit);
    Task<long> AddAsync(TAdd entity);
    Task<T> UpdateAsync(TUpdate entity, long id);
    Task AddRangeAsync(IEnumerable<TAdd> entities);
    Task RemoveAsync(long id);
    Task RemoveRangeAsync(IEnumerable<long> ids);
}
