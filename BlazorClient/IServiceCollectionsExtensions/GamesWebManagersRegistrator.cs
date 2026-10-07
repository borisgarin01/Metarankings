using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Games.Collections;
using Domain.RequestsModels.Games.Developers;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Localizations;
using Domain.RequestsModels.Games.Platforms;
using Domain.RequestsModels.Games.Publishers;
using WebManagers;
using WebManagers.Derived.CriticsReviews;
using WebManagers.Derived.Games;
using WebManagers.Derived.Waitings;

namespace BlazorClient.IServiceCollectionsExtensions;

public static class GamesWebManagersRegistrator
{
    public static IServiceCollection AddGamesWebManagers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel>, DevelopersWebManager>()
            .AddSingleton<IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel>, GenresWebManager>()
            .AddSingleton<IWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel>, LocalizationsWebManager>()
            .AddSingleton<IWebManager<Platform, AddPlatformModel, UpdatePlatformModel>, PlatformsWebManager>()
            .AddSingleton<IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel>, PublishersWebManager>()
            .AddSingleton<GamesWebManager>()
            .AddSingleton<IWebManager<Game, AddGameModel, UpdateGameModel>>(serviceProvider => serviceProvider.GetRequiredService<GamesWebManager>())
            .AddSingleton<IWebManager<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel>, GamesCollectionsWebManager>()
            .AddSingleton<IWebManager<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel>, GamesCollectionsItemsWebManager>()
            .AddSingleton<GamesPlayersReviewsShiftsWebManager>()
            .AddSingleton<GamesWaitingsWebManager>()
            .AddSingleton<GamesCriticsReviewsWebManager>();

        return serviceCollection;
    }
}
