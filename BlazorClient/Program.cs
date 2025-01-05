using BlazorClient;
using Blazored.LocalStorage;
using Domain;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAutoMapper(typeof(AppMappingProfile));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://192.168.1.102:5001") });

await builder.Build().RunAsync();
