using API.Auth;
using API.Hubs;
using API.IServiceCollectionExtensions;
using Data.Migrations;
using IdentityLibrary.DTOs;
using IdentityLibrary.Migrations;
using IdentityLibrary.Repositories;
using IdentityLibrary.Telegram;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Scalar.AspNetCore;
using Settings;

namespace API;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = Convert.ToBoolean(builder.Configuration["TokenValidationParameters:RequireExpirationTime"]),
            RequireSignedTokens = Convert.ToBoolean(builder.Configuration["TokenValidationParameters:RequireSignedTokens"]),
            ValidateIssuerSigningKey = Convert.ToBoolean(builder.Configuration["TokenValidationParameters:ValidateIssuerSigningKey"]),
            ValidateIssuer = Convert.ToBoolean(builder.Configuration["TokenValidationParameters:ValidateIssuer"]),
            ValidIssuer = builder.Configuration["TokenValidationParameters:ValidIssuer"],
            ValidateAudience = Convert.ToBoolean(builder.Configuration["TokenValidationParameters:ValidateAudience"]),
            ValidAudience = builder.Configuration["TokenValidationParameters:Audience"],
            ValidateLifetime = Convert.ToBoolean(builder.Configuration["TokenValidationParameters:ValidateLifetime"]),
            ClockSkew = TimeSpan.FromSeconds(Convert.ToInt64(builder.Configuration["TokenValidationParameters:ClockSkew"])),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenValidationParameters:IssuerSigningKey"]))
        };

        builder.Services.Configure<TokenValidationParameters>(builder.Configuration.GetSection(nameof(TokenValidationParameters)));

        builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection(nameof(AuthSettings)));

        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));

        builder.Services.AddLogging();
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            // Known networks for Docker
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //if you dont use Jwt i think you can just delete this line
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["AuthSettings:Authority"];
            options.Audience = builder.Configuration["AuthSettings:Audience"];
            options.ClaimsIssuer = builder.Configuration["AuthSettings:Issuer"];
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = tokenValidationParameters;
            options.SaveToken = true;
        }).AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = builder.Configuration["AuthSettings:Google:ClientId"];
            googleOptions.ClientSecret = builder.Configuration["AuthSettings:Google:ClientSecret"];
            googleOptions.CallbackPath = "/api/Auth/callback-uri"; // Match your controller route
            googleOptions.SaveTokens = true;
            googleOptions.Scope.Add("email");
            googleOptions.Scope.Add("profile");
            googleOptions.ClaimActions.MapJsonKey("picture","picture");
            googleOptions.ClaimActions.MapJsonKey("email","email");
            googleOptions.ClaimActions.MapJsonKey("name","name");
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", options =>
            {
                options.RequireRole("Admin");
            });
            options.AddPolicy("AuthorizedWithEmailConfirmed", options =>
            {
                options.RequireAuthenticatedUser();
                options.RequireClaim("EmailConfirmed", true.ToString());
            });
        });

        builder.Services.AddControllers(options => options.EnableEndpointRouting = false)
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        builder.Services.AddSignalR();

        builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                ["application/octet-stream"]);
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorFrontend", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        builder.Services.RegisterRepositories(builder.Configuration);
        builder.Services.RegisterFilesDataReaders();

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Tokens.EmailConfirmationTokenProvider = "Email";
            options.Tokens.PasswordResetTokenProvider = "Email";
        }).AddUserStore<UsersStore>()
          .AddRoleStore<RolesStore>()
          .AddTokenProvider<EmailTokenProvider<ApplicationUser>>("Email")
          .AddDefaultTokenProviders();

        builder.Services.AddSingleton<TelegramAuthenticator>();

        var app = builder.Build();

        app.UseForwardedHeaders();

        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseDeveloperExceptionPage();
        }

        app.UseBlazorFrameworkFiles();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.AddPreferredSecuritySchemes(["Bearer"]);
            options.AddHttpAuthentication("Bearer", bearer =>
            {
                bearer.Token = "Token";
            });
        });

        app.MapControllers();

        app.UseResponseCompression();

        app.MapHub<ChatHub>("/chathub");

        app.MapFallbackToFile("index.html");

        using (var serviceProvider = CreateServices(builder.Configuration))
        using (var scope = serviceProvider.CreateScope())
        {
            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            UpdateDatabase(scope.ServiceProvider);
        }

        // Use CORS middleware
        app.UseCors("AllowBlazorFrontend");

        app.Run();
    }

    private static ServiceProvider CreateServices(ConfigurationManager configurationManager)
    {
        return new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add SQLite support to FluentMigrator
                .AddPostgres()
                // Set the connection string
                .WithGlobalConnectionString(configurationManager.GetConnectionString("PostgresConnection"))
                // Define the assembly containing the migrations, maintenance migrations and other customizations
                .ScanIn(typeof(CreateGamesTableMigration).Assembly, typeof(CreateApplicationRolesTableMigration).Assembly).For.Migrations())
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