using Data.Repositories.Classes.Derived.Games;
using Data.Repositories.Classes.Derived.Movies;
using Data.Repositories.Interfaces;
using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.Movies;
using Domain.RequestsModels.Games.Developers;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Localizations;
using Domain.RequestsModels.Games.Platforms;
using Domain.RequestsModels.Games.Publishers;
using Domain.RequestsModels.Movies.MoviesDirectors;
using Domain.Reviews;

namespace API.IServiceCollectionExtensions;

public static class RepositoriesRegistrator
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        string metarankingsConnectionString = configuration.GetConnectionString("MetarankingsConnection");

        services.AddScoped<IRepository<Developer, AddDeveloperModel, UpdateDeveloperModel>, DevelopersRepository>(instance => new DevelopersRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<Genre, AddGenreModel, UpdateGenreModel>, GamesGenresRepository>(instance => new GamesGenresRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<Platform, AddPlatformModel, UpdatePlatformModel>, PlatformsRepository>(instance => new PlatformsRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<Localization, AddLocalizationModel, UpdateLocalizationModel>, LocalizationsRepository>(instance => new LocalizationsRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<Publisher, AddPublisherModel, UpdatePublisherModel>, PublishersRepository>(instance => new PublishersRepository(metarankingsConnectionString));

        services.AddScoped(instance => new GamesRepository(metarankingsConnectionString));

        services.AddScoped<ILocalizationsRepository>(instance => new LocalizationsRepository(metarankingsConnectionString));

        services.AddScoped<IMoviesRepository, MoviesRepository>(instance => new MoviesRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel>, MoviesDirectorsRepository>(instance => new MoviesDirectorsRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel>, MoviesGenresRepository>(instance => new MoviesGenresRepository(metarankingsConnectionString));

        services.AddScoped<IRepository<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>, MoviesStudiosRepository>(instance => new MoviesStudiosRepository(metarankingsConnectionString));

        services.AddScoped(instance => new GamesPlayersReviewsRepository(metarankingsConnectionString));

        services.AddScoped<IMoviesViewersReviewsRepository, MoviesViewersReviewsRepository>(instance => new MoviesViewersReviewsRepository(metarankingsConnectionString));

        return services;
    }
}
