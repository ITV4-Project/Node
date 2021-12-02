using Microsoft.Extensions.DependencyInjection;
using NodeNetworking.NodeNetworking.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetworking
{
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
}
