using NodeWebApi.Dtos.Wallets;
using NodeWebApi.Entities;

namespace NodeWebApi.Repositories
{
    public static class Extensions
    {
        public static WalletDto AsDto(this Wallet wallet)
        {
            return new WalletDto
            {
                Id = wallet.Id,
                CreationDate = wallet.CreationDate,
                PublicKey = wallet.PublicKey
            };
        }
    }
}
