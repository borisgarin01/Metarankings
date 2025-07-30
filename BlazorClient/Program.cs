using BlazorClient.Auth;

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
        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://172.16.1.62:5001") });

        await builder.Build().RunAsync();
    }
}