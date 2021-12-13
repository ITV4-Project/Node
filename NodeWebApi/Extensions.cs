using NodeWebApi.Dtos.Transactions;
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

        public static TransactionDto AsDto(this Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                Version = transaction.Version,
                CreationDate = transaction.CreationDate,
                Name = transaction.Name,
                MerkleHash = transaction.MerkleHash,
                Input = transaction.Input,
                Amount = transaction.Amount,
                Output = transaction.Output,
                Delegate = transaction.Delegate,
                Signature = transaction.Signature
            };
        }
    }
}
