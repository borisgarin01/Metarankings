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
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

        TokenValidationParameters tokenValidationParameters = new()
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

        _ = builder.Services.Configure<TokenValidationParameters>(builder.Configuration.GetSection(nameof(TokenValidationParameters)));

        _ = builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection(nameof(AuthSettings)));

        _ = builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));

        _ = builder.Services.AddLogging();
        _ = builder.Logging.ClearProviders();
        _ = builder.Logging.AddConsole();

        _ = builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            // Known networks for Docker
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        _ = builder.Services.AddAuthentication(options =>
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
            googleOptions.ClaimActions.MapJsonKey("picture", "picture");
            googleOptions.ClaimActions.MapJsonKey("email", "email");
            googleOptions.ClaimActions.MapJsonKey("name", "name");
        });

        _ = builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", options =>
            {
                _ = options.RequireRole("Admin");
            });
            options.AddPolicy("AuthorizedWithEmailConfirmed", options =>
            {
                _ = options.RequireAuthenticatedUser();
                _ = options.RequireClaim("EmailConfirmed", true.ToString());
            });
        });

        _ = builder.Services.AddScoped<AuthTokenGenerator>();

        _ = builder.Services.AddControllers(options => options.EnableEndpointRouting = false)
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        _ = builder.Services.AddEndpointsApiExplorer();

        _ = builder.Services.AddOpenApi("v1", options =>
        {
            _ = options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        _ = builder.Services.AddSignalR();

        _ = builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                ["application/octet-stream"]);
        });

        _ = builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorFrontend", builder =>
            {
                _ = builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        _ = builder.Services.RegisterRepositories(builder.Configuration);
        _ = builder.Services.RegisterFilesDataReaders();

        _ = builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Tokens.EmailConfirmationTokenProvider = "Email";
            options.Tokens.PasswordResetTokenProvider = "Email";
        }).AddUserStore<UsersStore>()
          .AddRoleStore<RolesStore>()
          .AddTokenProvider<EmailTokenProvider<ApplicationUser>>("Email")
          .AddDefaultTokenProviders();

        _ = builder.Services.AddSingleton<TelegramAuthenticator>();

        WebApplication app = builder.Build();

        _ = app.UseForwardedHeaders();

        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            _ = app.UseDeveloperExceptionPage();
        }

        _ = app.UseBlazorFrameworkFiles();

        _ = app.UseStaticFiles();

        _ = app.UseRouting();

        _ = app.UseAuthentication();
        _ = app.UseAuthorization();

        _ = app.MapOpenApi();
        _ = app.MapScalarApiReference(options =>
        {
            _ = options.AddPreferredSecuritySchemes(["Bearer"]);
            _ = options.AddHttpAuthentication("Bearer", bearer =>
            {
                bearer.Token = "Token";
            });
        });

        _ = app.MapControllers();

        _ = app.UseResponseCompression();

        _ = app.MapHub<ChatHub>("/chathub");

        _ = app.MapFallbackToFile("index.html");

        using (ServiceProvider serviceProvider = CreateServices(builder.Configuration))
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            UpdateDatabase(scope.ServiceProvider);
        }

        // Use CORS middleware
        _ = app.UseCors("AllowBlazorFrontend");

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
        IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.ListMigrations();
        // Execute the migrations
        runner.MigrateUp();
    }
}