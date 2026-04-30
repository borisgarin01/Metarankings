using BlazorClient.Auth;
using Domain.Games;
using Domain.Games.Collections;
using Domain.Movies;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Games.Collections;
using Domain.RequestsModels.Games.Developers;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Localizations;
using Domain.RequestsModels.Games.Platforms;
using Domain.RequestsModels.Games.Publishers;
using Domain.RequestsModels.Movies.Movies;
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
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("Admin", options => { options.RequireRole("Admin"); });
        });

        builder.Services.AddScoped<IWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel>, DevelopersWebManager>();
        builder.Services.AddScoped<IWebManager<Genre, AddGenreModel, UpdateGenreModel>, GenresWebManager>();
        builder.Services.AddScoped<IWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel>, LocalizationsWebManager>();
        builder.Services.AddScoped<IWebManager<Platform, AddPlatformModel, UpdatePlatformModel>, PlatformsWebManager>();
        builder.Services.AddScoped<IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel>, PublishersWebManager>();
        builder.Services.AddScoped<IWebManager<Game, AddGameModel, UpdateGameModel>, GamesWebManager>();
        builder.Services.AddScoped<IWebManager<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel>, GamesCollectionsWebManager>();

        builder.Services.AddScoped<IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel>, MoviesDirectorsWebManager>();
        builder.Services.AddScoped<IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel>, MoviesGenresWebManager>();
        builder.Services.AddScoped<IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>, MoviesStudiosWebManager>();
        builder.Services.AddScoped<IWebManager<Movie, AddMovieModel, UpdateMovieModel>, MoviesWebManager>();

        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["HttpClientSettings:BaseUrl"]) });
        builder.Services.AddScoped<IAuthService, AuthService>();

        await builder.Build().RunAsync();
    }
}