using BlazorClient;
using Domain;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAutoMapper(typeof(AppMappingProfile));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7148") });

await builder.Build().RunAsync();
