using BlazorClient;
using BlazorClient.Auth;
using BlazorClient.IServiceCollectionsExtensions;
using Blazored.Toast;
using Blazored.Toast.Services;

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

        var baseUrl = builder.Configuration["HttpClientSettings:BaseUrl"];
        // Регистрируем обработчик
        builder.Services.AddScoped<JwtAuthorizationHandler>();

        // Именованный клиент для всех авторизованных запросов
        builder.Services.AddHttpClient("AuthorizedClient")
            .AddHttpMessageHandler<JwtAuthorizationHandler>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });

        // 🔥 ВАЖНО: Регистрируем AuthService через фабрику с именованным клиентом
        builder.Services.AddScoped<IAuthService>(sp =>
        {
            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var client = clientFactory.CreateClient("AuthorizedClient");
            var localStorage = sp.GetRequiredService<ILocalStorageService>();
            var logger = sp.GetRequiredService<ILogger<AuthService>>();
            var toastService = sp.GetRequiredService<IToastService>();
            return new AuthService(client, localStorage, logger, toastService);
        });

        // Регистрируем все WebManager
        builder.Services.RegisterWebManagers();

        // JwtAuthenticationStateProvider
        builder.Services.AddScoped<JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(
            provider => provider.GetRequiredService<JwtAuthenticationStateProvider>());

        _ = builder.Services.AddBlazoredToast();

        builder.Logging.SetMinimumLevel(LogLevel.Debug);

        await builder.Build().RunAsync();
    }
}