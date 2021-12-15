using NodeRepository.Entities;

namespace NodeRepository.Repositories.Wallets;

public interface IWalletsRepository
{
    Wallet GetWallet(Guid id);
    IEnumerable<Wallet> GetWallets();
    void CreateWallet(Wallet wallet);
    void UpdateWallet(Wallet wallet);
    void DeleteWallet(Guid id);
}