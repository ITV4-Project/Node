using NodeWebApi.Dtos.Transactions;
using Core;

namespace NodeWebApi
{
    public static class Extensions
    {

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
