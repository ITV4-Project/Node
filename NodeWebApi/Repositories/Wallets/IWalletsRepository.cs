using NodeWebApi.Entities;

namespace NodeWebApi.Repositories.Wallets
{
    public interface IWalletsRepository
    {
        Wallet GetWallet(Guid id);
        IEnumerable<Wallet> GetWallets();
        void CreateWallet(Wallet wallet);
        void UpdateWallet(Wallet wallet);
        void DeleteWallet(Guid id);
    }
}