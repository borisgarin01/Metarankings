namespace Metarankings.Repositories.Interfaces.Base
{
    public interface IRepository<T> where T : class
    {
        Task<IQueryable<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(Predicate<T>predicateToDelete);
    }
}
