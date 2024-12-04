using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories.Interfaces;
public interface IMoviesGenresRepository
{
    public Task AddAsync(Genre genre);
    public Task<IEnumerable<Genre>> GetAllAsync();
}
