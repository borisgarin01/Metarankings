using BlazorClient.Auth;
using Blazored.Toast;
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

namespace BlazorClient;

internal class Program
{
    private static async Task Main(string[] args)
    {
        WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        _ = builder.Services.AddBlazoredLocalStorage();
        _ = builder.Services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("Admin", options => { _ = options.RequireRole("Admin"); });
        });

        _ = builder.Services.AddScoped<IWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel>, DevelopersWebManager>();
        _ = builder.Services.AddScoped<IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel>, GenresWebManager>();
        _ = builder.Services.AddScoped<IWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel>, LocalizationsWebManager>();
        _ = builder.Services.AddScoped<IWebManager<Platform, AddPlatformModel, UpdatePlatformModel>, PlatformsWebManager>();
        _ = builder.Services.AddScoped<IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel>, PublishersWebManager>();
        _ = builder.Services.AddScoped<GamesWebManager>();
        _ = builder.Services.AddScoped<IWebManager<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel>, GamesCollectionsWebManager>();
        _ = builder.Services.AddScoped<IWebManager<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel>, GamesCollectionsItemsWebManager>();
        _ = builder.Services.AddScoped<GamesPlayersReviewsShiftsWebManager>();

        _ = builder.Services.AddScoped<IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel>, MoviesDirectorsWebManager>();
        _ = builder.Services.AddScoped<IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel>, MoviesGenresWebManager>();
        _ = builder.Services.AddScoped<IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>, MoviesStudiosWebManager>();
        _ = builder.Services.AddScoped<MoviesWebManager>();
        _ = builder.Services.AddScoped<IWebManager<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel>, MoviesCollectionsWebManager>();
        _ = builder.Services.AddScoped<IWebManager<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel>, MoviesCollectionsItemsWebManager>();

        _ = builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

        _ = builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["HttpClientSettings:BaseUrl"]) });
        _ = builder.Services.AddScoped<IAuthService, AuthService>();
        _ = builder.Services.AddScoped<JwtAuthenticationStateProvider>();
        _ = builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<JwtAuthenticationStateProvider>());

        _ = builder.Services.AddBlazoredToast();

        await builder.Build().RunAsync();
    }
}