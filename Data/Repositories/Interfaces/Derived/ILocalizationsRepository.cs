using Domain.Games;

namespace Data.Repositories.Interfaces.Derived;
public interface ILocalizationsRepository : IRepository<Localization>
{
    Task<Localization> GetByPlatformAsync(long id, long platformId);
}
