using Data.Repositories.Classes.Derived;
using Data.Repositories.Interfaces;
using Domain;

namespace API.IServiceCollectionExtensions;

public static class RepositoriesRegistrator
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRepository<Developer>, DevelopersRepository>(instance => new DevelopersRepository(configuration.GetConnectionString("MetarankingsConnection")));

        services.AddScoped<IRepository<Genre>, GenresRepository>(instance => new GenresRepository(configuration.GetConnectionString("MetarankingsConnection")));

        services.AddScoped<IRepository<Platform>, PlatformsRepository>(instance => new PlatformsRepository(configuration.GetConnectionString("MetarankingsConnection")));

        services.AddScoped<IRepository<Localization>, LocalizationsRepository>(instance => new LocalizationsRepository(configuration.GetConnectionString("MetarankingsConnection")));

        services.AddScoped<IRepository<Publisher>, PublishersRepository>(instance => new PublishersRepository(configuration.GetConnectionString("MetarankingsConnection")));

        return services;
    }
}
