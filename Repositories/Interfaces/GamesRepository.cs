using Metarankings.Models;
using Metarankings.Repositories.Interfaces.Derived;

namespace Metarankings.Repositories.Classes
{
    public class GamesRepository : IGamesRepository
    {
        public Task AddAsync(Game entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(Predicate<Game> predicateToDelete)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Game>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Game> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Game entity)
        {
            throw new NotImplementedException();
        }
    }
}
