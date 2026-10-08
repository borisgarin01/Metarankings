namespace Data.Repositories.Interfaces;
public interface IRepository<T, TAdd, TUpdate>
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> GetAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default);
    Task<long> AddAsync(TAdd entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(TUpdate entity, long id, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TAdd> entities, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
    Task RemoveRangeAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
}
