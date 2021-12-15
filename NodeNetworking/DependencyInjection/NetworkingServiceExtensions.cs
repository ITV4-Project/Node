using Microsoft.Extensions.DependencyInjection;
using System;

namespace NodeNetworking.DependencyInjection;

public static class NetworkingServiceExtensions
{
    public static IServiceCollection AddGossipProtocol(
        this IServiceCollection services,
        Action<GossipProtocolOptions> setupAction = null)
    {
        services.AddHostedService<NetworkingService>();

        if (setupAction != null) services.ConfigureGossipProtocol(setupAction);

        return services;
    }

    public static void ConfigureGossipProtocol(
        this IServiceCollection services,
        Action<GossipProtocolOptions> setupAction)
    {
        services.Configure(setupAction);
    }
}
