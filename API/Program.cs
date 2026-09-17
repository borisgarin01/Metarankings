using API.Auth;
using API.Hubs;
using API.IServiceCollectionExtensions;
using AspNet.Security.OAuth.VkId;
using Data.Migrations;
using IdentityLibrary.DTOs;
using IdentityLibrary.Migrations;
using IdentityLibrary.Repositories;
using IdentityLibrary.Repositories.Tokens.RefreshTokens.Classes;
using IdentityLibrary.Repositories.Tokens.RefreshTokens.Interfaces;
using IdentityLibrary.Services.Classes;
using IdentityLibrary.Services.Interfaces;
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenValidationParameters:IssuerSigningAccessKey"]))
        };

        builder.Configuration.AddEnvironmentVariables();

        _ = builder.Services.Configure<TokenValidationParameters>(builder.Configuration.GetSection(nameof(TokenValidationParameters)));

        _ = builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection(nameof(AuthSettings)));

        _ = builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));

        _ = builder.Services.AddLogging();
        _ = builder.Logging.ClearProviders();
        _ = builder.Logging.AddConsole();

        _ = builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
                                     | ForwardedHeaders.XForwardedProto
                                     | ForwardedHeaders.XForwardedHost;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        _ = builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; //if you dont use Jwt i think you can just delete this line
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["AuthSettings:Authority"];
            options.Audience = builder.Configuration["AuthSettings:Audience"];
            options.ClaimsIssuer = builder.Configuration["AuthSettings:Issuer"];
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = tokenValidationParameters;
            options.SaveToken = true;
        }).
        AddVkId(vkOptions =>
        {
            vkOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            vkOptions.ClientId = builder.Configuration["AuthSettings:VkId:ClientId"];
            vkOptions.ClientSecret = builder.Configuration["AuthSettings:VkId:ClientSecret"];
            vkOptions.AuthorizationEndpoint = builder.Configuration["AuthSettings:VkId:AuthUri"];
            vkOptions.TokenEndpoint = builder.Configuration["AuthSettings:VkId:TokenUri"];
            vkOptions.CallbackPath = builder.Configuration["AuthSettings:VkId:CallbackPath"];
            vkOptions.CorrelationCookie.SameSite = SameSiteMode.None;
            vkOptions.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
        })
        .AddCookie()
        .AddCookie("cookie");

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

        _ = builder.Services.AddScoped<TwoFactorAuthEmailProcessor>();

        builder.Services.AddScoped<ITokensService, TokensService>();


        _ = builder.Services.AddControllers(options => options.EnableEndpointRouting = false)
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        _ = builder.Services.AddEndpointsApiExplorer();

        _ = builder.Services.AddOpenApi();

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

        builder.Services.AddScoped<IRefreshTokensRepository>(sp =>
            new RefreshTokensRepository(builder.Configuration.GetConnectionString("PostgresConnection")));

        WebApplication app = builder.Build();

        app.Use(async (ctx, next) =>
        {
            Console.WriteLine($"Scheme={ctx.Request.Scheme} Host={ctx.Request.Host} XFP={ctx.Request.Headers["X-Forwarded-Proto"]}");
            await next();
        });

        app.UseForwardedHeaders();

        app.Use(async (ctx, next) =>
        {
            Console.WriteLine($"[AFTER FWD] Scheme={ctx.Request.Scheme} Host={ctx.Request.Host}");
            await next();
        });

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
            options
                .WithTitle("MetaRankings API")
                .WithTheme(ScalarTheme.Purple)
                .WithDarkModeToggle(true)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

            // В .NET 10 используется другой метод
            options.AddPreferredSecuritySchemes("Bearer");
        });

        _ = app.MapControllers();

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