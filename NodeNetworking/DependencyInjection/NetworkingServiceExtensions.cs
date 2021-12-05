using NodeNetworking;
using NodeNetworking.NodeNetworking.DependencyInjection;
using System;

namespace Microsoft.Extensions.DependencyInjection;

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
