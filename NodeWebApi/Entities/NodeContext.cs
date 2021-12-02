using Microsoft.EntityFrameworkCore;

namespace NodeWebApi.Entities;

public class NodeContext : DbContext
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
}
