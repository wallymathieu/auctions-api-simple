using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wallymathieu.Auctions.Infrastructure.Services.Cache;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

public static class ServiceExtensions
{
    private static IServiceCollection AddAuctionServicesImplementation(this IServiceCollection services)
    {
        services.TryAddSingleton<ITime, Time>();
        services.TryAddSingleton<Mapper>();
        services.TryAddScoped<CreateAuctionCommandHandler>();
        services.TryAddScoped<CreateBidCommandHandler>();
        return services;
    }

    public static IServiceCollection AddAuctionServicesNoCache(this IServiceCollection services)
    {
        return AddAuctionServicesImplementation(services)
            .AddScoped<ICreateAuctionCommandHandler>(c => c.GetRequiredService<CreateAuctionCommandHandler>())
            .AddScoped<ICreateBidCommandHandler>(c => c.GetRequiredService<CreateBidCommandHandler>());
    }

    public static IServiceCollection AddAuctionServicesCached(this IServiceCollection services)
    {
        return AddAuctionServicesImplementation(services)
            .AddScoped<ICreateAuctionCommandHandler>(c=>new CacheAwareCreateAuctionCommandHandler(c.GetRequiredService<CreateAuctionCommandHandler>(), c.GetRequiredService<IDistributedCache>()))
            .AddScoped<ICreateBidCommandHandler>(c=>new CacheAwareCreateBidCommandHandler(c.GetRequiredService<CreateBidCommandHandler>(), c.GetRequiredService<IDistributedCache>()))
            ;
    }
}