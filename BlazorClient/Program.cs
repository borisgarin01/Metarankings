using BlazorClient.Auth;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorClient;
internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore();

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://192.168.1.101:5001") });

        await builder.Build().RunAsync();
    }
}