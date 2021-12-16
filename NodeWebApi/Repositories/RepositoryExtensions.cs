using Microsoft.EntityFrameworkCore;
using NodeWebApi.Entities;
using NodeWebApi.Repositories.Transactions;
using NodeWebApi.Repositories.Wallets;

namespace NodeWebApi.Repositories;

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

