using Data.Repositories.Interfaces;

namespace Data.Repositories.Classes;

public abstract class Repository
{
    public string ConnectionString { get; }

    public Repository(string connectionString)
    {
        ConnectionString = connectionString;
    }

    protected NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(ConnectionString);
    }
}

/// <summary>
/// Базовый репозиторий с поэлементной реализацией массовых операций по умолчанию.
/// </summary>
public abstract class Repository<T, TAdd, TUpdate> : Repository, IRepository<T, TAdd, TUpdate>
{
    protected Repository(string connectionString) : base(connectionString)
    {
    }

    public abstract Task<IEnumerable<T>> GetAllAsync();

    public abstract Task<T> GetAsync(long id);

    public abstract Task<IEnumerable<T>> GetAsync(long offset, long limit);

    public abstract Task<long> AddAsync(TAdd entity);

    public abstract Task<T> UpdateAsync(TUpdate entity, long id);

    public abstract Task RemoveAsync(long id);

    public virtual async Task AddRangeAsync(IEnumerable<TAdd> entities)
    {
        foreach (TAdd entity in entities)
            await AddAsync(entity);
    }

    public virtual async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (long id in ids)
            await RemoveAsync(id);
    }
}
