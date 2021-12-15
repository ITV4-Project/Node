using Microsoft.Extensions.DependencyInjection;
using NodeRepository.Repositories.Transactions;
using NodeRepository.Repositories.Wallets;

namespace NodeRepository.Repositories;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<ITransactionsRepository, TransactionsRepository>();
        services.AddScoped<IWalletsRepository, WalletsRepository>();

        return services;
    }
}

