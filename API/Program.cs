using API.IServiceCollectionExtensions;
using Data.Migrations;
using IdentityLibrary.DTOs;
using IdentityLibrary.Migrations;
using IdentityLibrary.Repositories;
using IdentityLibrary.Telegram;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = false,
            RequireSignedTokens = false,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Auth:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Auth:Audience"],
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Auth:Secret"]))
        };

        builder.Services.AddLogging();
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //if you dont use Jwt i think you can just delete this line
        }).AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["Auth:Authority"];
            options.Audience = builder.Configuration["Auth:Audience"];
            options.ClaimsIssuer = builder.Configuration["Auth:Issuer"];
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = tokenValidationParameters;
            options.SaveToken = true;
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

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MyAPI",
                Version = "v1"
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference=new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });


        builder.Services.AddControllers(options => options.EnableEndpointRouting = false)
            .AddJsonOptions(options =>
         {
             options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
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

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddUserStore<UsersStore>()
            .AddRoleStore<RolesStore>()
            .AddDefaultTokenProviders();

        builder.Services.AddSingleton<TelegramAuthenticator>();

        builder.Services.AddHostedService<NotificationsBackgroundService>();

        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        var app = builder.Build();

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

        app.MapControllers();

        app.MapFallbackToFile("index.html");

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

        app.Run();
    }

    private static ServiceProvider CreateServices(ConfigurationManager configurationManager)
    {
        return new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add SQLite support to FluentMigrator
                .AddSqlServer()
                // Set the connection string
                .WithGlobalConnectionString(configurationManager.GetConnectionString("MetarankingsConnection"))
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
