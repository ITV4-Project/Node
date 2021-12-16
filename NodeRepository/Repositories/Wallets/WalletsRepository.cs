using NodeRepository.Entities;
using System.Text;

namespace NodeRepository.Repositories.Wallets;

public class WalletsRepository : IWalletsRepository
{
    private readonly NodeContext _nodeContext;
    public WalletsRepository(
        NodeContext context)
    {
        _nodeContext = context;
    }

    private static readonly List<Wallet> wallets = new()
    {
        new Wallet { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, PublicKey = Encoding.ASCII.GetBytes("XTTMTuLYMrvbsvtR9h0MLBPQLLZNLB8LXTTMNrVNLBPLiw4lCyCXMM9RKv") },
        new Wallet { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, PublicKey = Encoding.ASCII.GetBytes("SOOHOpGTHmqwnqoM9c0HGWKLGGUIGW8GSOOHImQIGWKGdr4gXtXSHH9MFq") },
        new Wallet { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, PublicKey = Encoding.ASCII.GetBytes("AWWPWxOBPuyevywU9k0POESTOOCQOE8OAWWPQuYQOESOlz4oFbFAPP9UNy") }
    };

    public IEnumerable<Wallet> GetWallets()
    {
        return wallets;
    }

    public Wallet GetWallet(Guid id)
    {
        return wallets.Where(wallet => wallet.Id == id).SingleOrDefault();
    }

    void IWalletsRepository.CreateWallet(Wallet wallet)
    {
        wallets.Add(wallet);
    }

    void IWalletsRepository.UpdateWallet(Wallet wallet)
    {
        var index = wallets.FindIndex(existingWallet => existingWallet.Id == wallet.Id);
        wallets[index] = wallet;
    }

    void IWalletsRepository.DeleteWallet(Guid id)
    {
        var index = wallets.FindIndex(existingWallet => existingWallet.Id == id);
        wallets.RemoveAt(index);
    }
}
