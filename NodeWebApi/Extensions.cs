using NodeWebApi.Dtos.Transactions;
using NodeWebApi.Dtos.Wallets;
using NodeRepository.Entities;
using Core;

namespace NodeWebApi
{
    public static class Extensions
    {
        public static WalletDto AsDto(this Wallet wallet)
        {
            return new WalletDto
            {
                Id = wallet.Id,
                CreationDate = wallet.CreationTime,
                PublicKey = wallet.PublicKey
            };
        }

        public static TransactionDto AsDto(this Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                Version = transaction.Version,
                CreationTime = transaction.CreationTime,
                MerkleHash = transaction.MerkleHash,
                Input = transaction.Input,
                Amount = transaction.Amount,
                Output = transaction.Output,
                IsDelegating = transaction.IsDelegating,
                Signature = transaction.Signature
            };
        }
    }
}
