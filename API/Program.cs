var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7280", "http://localhost:5152", "http://localhost:52856")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Use CORS middleware
app.UseCors("AllowBlazorFrontend");

app.UseAuthorization();
app.MapControllers();

app.Run();