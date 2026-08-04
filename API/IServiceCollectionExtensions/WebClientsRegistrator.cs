using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using Domain.RequestsModels.Games.Developers;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Localizations;
using Domain.RequestsModels.Games.Platforms;
using Domain.RequestsModels.Games.Publishers;
using WebManagers;
using WebManagers.Derived.Games;

namespace API.IServiceCollectionExtensions;

public static class WebClientsRegistrator
{
    public static IServiceCollection AddGamesWebManagers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel>, DevelopersWebManager>()
            .AddSingleton<IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel>, GenresWebManager>()
            .AddSingleton<IWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel>, LocalizationsWebManager>()
            .AddSingleton<IWebManager<Platform, AddPlatformModel, UpdatePlatformModel>, PlatformsWebManager>()
            .AddSingleton<IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel>, PublishersWebManager>()
            .AddSingleton<GamesWebManager>()
            .AddSingleton<IWebManager<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel>, GamesCollectionsWebManager>()
            .AddSingleton<IWebManager<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel>, GamesCollectionsItemsWebManager>()
            .AddSingleton<GamesPlayersReviewsShiftsWebManager>();

        return serviceCollection;
    }
}
