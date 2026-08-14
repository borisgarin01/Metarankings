using Domain.Games;
using Domain.RequestsModels.Games.Developers;

namespace Data.Repositories.Interfaces.Derived;

public interface IDevelopersRepository : IRepository<Developer, AddDeveloperModel, UpdateDeveloperModel>
{
    public Task<Developer> GetByNameAsync(string name);
}
