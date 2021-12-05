using Microsoft.EntityFrameworkCore;
using NodeWebApi.Entities;
using NodeWebApi.Repositories.Transactions;
using NodeWebApi.Repositories.Wallets;

namespace Microsoft.Extensions.DependencyInjection;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddSingleton<ITransactionsRepository, TransactionsRepository>();
        services.AddSingleton<IWalletsRepository, WalletsRepository>();

        return services;
    }
}

