using API.Caching;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace API.IServiceCollectionExtensions;

public static class CachingRegistrator
{
    private static readonly TimeSpan PublicReadExpiration = TimeSpan.FromMinutes(10);

    private const string RedisInstanceName = "metarankings:";

    /// <summary>
    /// Регистрирует IDistributedCache и output-кэш ответов API.
    /// С ConnectionStrings:RedisConnection оба хранятся в Redis, без неё — в памяти (локальная разработка).
    /// </summary>
    public static IServiceCollection RegisterCaching(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddOutputCache(options =>
        {
            options.AddPolicy(CachePolicies.PublicRead, new PublicReadCachePolicy(PublicReadExpiration));
        });

        string? redisConnectionString = configuration.GetConnectionString("RedisConnection");
        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            _ = services.AddDistributedMemoryCache();
            return services;
        }

        _ = services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = RedisInstanceName;
        });

        _ = services.AddStackExchangeRedisOutputCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = RedisInstanceName + "output:";
        });

        DecorateOutputCacheStoreWithResilience(services);

        return services;
    }

    private static void DecorateOutputCacheStoreWithResilience(IServiceCollection services)
    {
        ServiceDescriptor storeDescriptor = services.Last(descriptor => descriptor.ServiceType == typeof(IOutputCacheStore));
        _ = services.RemoveAll<IOutputCacheStore>();

        _ = services.AddSingleton<IOutputCacheStore>(serviceProvider =>
        {
            IOutputCacheStore inner = storeDescriptor switch
            {
                { ImplementationInstance: IOutputCacheStore instance } => instance,
                { ImplementationFactory: not null } => (IOutputCacheStore)storeDescriptor.ImplementationFactory(serviceProvider),
                _ => (IOutputCacheStore)ActivatorUtilities.CreateInstance(serviceProvider, storeDescriptor.ImplementationType!)
            };

            return new ResilientOutputCacheStore(inner, serviceProvider.GetRequiredService<ILogger<ResilientOutputCacheStore>>());
        });
    }
}
