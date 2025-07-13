using Domain.Games;
using System.Threading.Tasks;

namespace Data.Repositories.Interfaces.Derived;
public interface ILocalizationsRepository : IRepository<Localization>
{
    public Task<Localization> GetByPlatformAsync(long id, long platformId);
}
