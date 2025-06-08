using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Classes.Derived.Movies;
using Data.Repositories.Interfaces;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.Movies;

namespace API.IServiceCollectionExtensions;

public static class RepositoriesRegistrator
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        string metarankingsConnectionString = configuration.GetConnectionString("MetarankingsConnection");

        services.AddScoped<IRepository<Developer>, DevelopersRepository>(instance => new DevelopersRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<Genre>, GamesGenresRepository>(instance => new GamesGenresRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<Platform>, PlatformsRepository>(instance => new PlatformsRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<Localization>, LocalizationsRepository>(instance => new LocalizationsRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<Publisher>, PublishersRepository>(instance => new PublishersRepository(metarankingsConnectionString));

        services.AddScoped(instance => new GamesRepository(metarankingsConnectionString));

        services.AddScoped<ILocalizationsRepository>(instance => new LocalizationsRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<MovieDirector>, MoviesDirectorsRepository>(instance => new MoviesDirectorsRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<MovieGenre>, MoviesGenresRepository>(instance => new MoviesGenresRepository(metarankingsConnectionString));

        return services;
    }
}
