using Microsoft.Extensions.DependencyInjection;
using Core.Database;
using Core;
using NodeNetworking.Buffering;

namespace NodeNetworking.DependencyInjection {
	public static class LedgerExtensions {
        public static IServiceCollection AddLedger(
        this IServiceCollection services) {
            services.AddScoped<ILedger, Ledger>();

            services.AddScoped<IBuffer<Transaction>, Buffer<Transaction>>();
            services.AddScoped<IBuffer<Block>, Buffer<Block>>();
            return services;
        }
    }

}
