using Microsoft.EntityFrameworkCore;

namespace NodeWebApi.Entities;

public class NodeContext : DbContext
{
    public NodeContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
}
