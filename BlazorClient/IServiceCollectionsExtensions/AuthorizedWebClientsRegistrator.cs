using Domain.Games;
using Domain.Games.Collections;
using Domain.Movies;
using Domain.Movies.Collections;
using Domain.RequestsModels.Games.Collections;
using Domain.RequestsModels.Games.Developers;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Localizations;
using Domain.RequestsModels.Games.Platforms;
using Domain.RequestsModels.Games.Publishers;
using Domain.RequestsModels.Movies.Collections;
using Domain.RequestsModels.Movies.MoviesDirectors;
using Domain.RequestsModels.Movies.MoviesGenres;
using Domain.RequestsModels.Movies.MoviesStudios;
using WebManagers;
using WebManagers.Derived.Games;
using WebManagers.Derived.Movies;

namespace BlazorClient.IServiceCollectionsExtensions;

public static class AuthorizedWebClientsRegistrator
{
    private static IServiceCollection AddAuthorized<TInterface, TImplementation>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.AddScoped<TInterface>(sp =>
        {
            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var client = clientFactory.CreateClient("AuthorizedClient");
            return ActivatorUtilities.CreateInstance<TImplementation>(sp, client);
        });
        return services;
    }

    private static IServiceCollection AddAuthorized<TImplementation>(
        this IServiceCollection services)
        where TImplementation : class
    {
        services.AddScoped<TImplementation>(sp =>
        {
            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var client = clientFactory.CreateClient("AuthorizedClient");
            return ActivatorUtilities.CreateInstance<TImplementation>(sp, client);
        });
        return services;
    }

    public static IServiceCollection RegisterWebManagers(this IServiceCollection serviceCollection)
    {
        // Games
        serviceCollection
            .AddAuthorized<IWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel>, DevelopersWebManager>()
            .AddAuthorized<IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel>, GenresWebManager>()
            .AddAuthorized<IWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel>, LocalizationsWebManager>()
            .AddAuthorized<IWebManager<Platform, AddPlatformModel, UpdatePlatformModel>, PlatformsWebManager>()
            .AddAuthorized<IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel>, PublishersWebManager>()
            .AddAuthorized<GamesWebManager>()
            .AddAuthorized<IWebManager<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel>, GamesCollectionsWebManager>()
            .AddAuthorized<IWebManager<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel>, GamesCollectionsItemsWebManager>()
            .AddAuthorized<GamesPlayersReviewsShiftsWebManager>();

        // Movies
        serviceCollection
            .AddAuthorized<IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel>, MoviesDirectorsWebManager>()
            .AddAuthorized<IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel>, MoviesGenresWebManager>()
            .AddAuthorized<IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>, MoviesStudiosWebManager>()
            .AddAuthorized<MoviesWebManager>()
            .AddAuthorized<IWebManager<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel>, MoviesCollectionsWebManager>()
            .AddAuthorized<IWebManager<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel>, MoviesCollectionsItemsWebManager>();

        return serviceCollection;
    }
}