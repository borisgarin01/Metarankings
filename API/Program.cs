using API.IServiceCollectionExtensions;
using Data.Migrations;
using FluentMigrator.Runner;
using System.Reflection;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

        builder.Services.AddControllers(options => options.EnableEndpointRouting = false)
            .AddJsonOptions(options =>
         {
             options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
         });

        builder.Services.AddSwaggerGen();

        // Add CORS services
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorFrontend", policy =>
            {
                policy.WithOrigins("https://localhost:7280", "http://localhost:5152", "http://localhost:52856", "https://192.168.1.102:5003")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        builder.Services.RegisterRepositories(builder.Configuration);
        builder.Services.RegisterValidators();

        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        var app = builder.Build();

        using (var serviceProvider = CreateServices(builder.Configuration))
        using (var scope = serviceProvider.CreateScope())
        {
            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            UpdateDatabase(scope.ServiceProvider);
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        // Use CORS middleware
        app.UseCors("AllowBlazorFrontend");

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    private static ServiceProvider CreateServices(ConfigurationManager configurationManager)
    {
        return new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add SQLite support to FluentMigrator
                .AddPostgres15_0()
                // Set the connection string
                .WithGlobalConnectionString(configurationManager.GetConnectionString("MetarankingsConnection"))
                // Define the assembly containing the migrations, maintenance migrations and other customizations
                .ScanIn(typeof(AddGamesTableMigration).Assembly).For.Migrations())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Build the service provider
            .BuildServiceProvider(false);
    }

    /// <summary>
    /// Update the database
    /// </summary>
    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.ListMigrations();
        // Execute the migrations
        runner.MigrateUp();
    }
}
