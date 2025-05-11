using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorClient;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

#if DEBUG
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7032") });
#else
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://192.168.1.102:5001") });
#endif

await builder.Build().RunAsync();
