using BlazorClient.Auth;
using Domain.Games;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Games.Developers;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Localizations;
using Domain.RequestsModels.Games.Platforms;
using Domain.RequestsModels.Games.Publishers;
using WebManagers;
using WebManagers.Derived;

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

        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://172.16.1.62:5001") });

        await builder.Build().RunAsync();
    }
}