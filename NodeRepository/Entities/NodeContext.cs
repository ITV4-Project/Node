using Microsoft.EntityFrameworkCore;
using Core;

namespace NodeRepository.Entities;

public class NodeContext : DbContext
{
    public NodeContext(DbContextOptions options) : base(options)
    {

    }
    public DbSet<Transaction> Transactions { get; set; } => Set<Transaction>();
    public DbSet<Wallet> Wallets { get; set; }
}
