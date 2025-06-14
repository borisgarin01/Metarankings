using IdentityLibrary.DTOs;
using IdentityLibrary.Repositories;
using Microsoft.AspNetCore.Identity;

namespace API.IServiceCollectionExtensions;

public static class IdentityRegistrator
{
    public static IServiceCollection RegisterIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        string metarankingsConnectionString = configuration.GetConnectionString("MetarankingsConnection");

        services.AddScoped<IUserStore<ApplicationUser>, UsersStore>(instance => new UsersStore(metarankingsConnectionString));
        services.AddScoped<IRoleStore<ApplicationRole>, RoleStore>(instance => new RoleStore(metarankingsConnectionString));

        services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddDefaultTokenProviders();

        return services;
    }
}
