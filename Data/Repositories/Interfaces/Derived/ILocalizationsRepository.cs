using Domain.Games;
using Domain.RequestsModels.Games.Localizations;

namespace Data.Repositories.Interfaces.Derived;
public interface ILocalizationsRepository : IRepository<Localization, AddLocalizationModel, UpdateLocalizationModel>
{
    Task<Localization> GetByPlatformAsync(long id, long platformId);
}
