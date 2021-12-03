using NodeWebApi.Entities;
using NodeWebApi.Repositories.Wallets;
using System.Text;

namespace NodeWebApi.Repositories.Transactions
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private static readonly List<Wallet> wallets = new()
        {
            new Wallet { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, PublicKey = Encoding.ASCII.GetBytes("XTTMTuLYMrvbsvtR9h0MLBPQLLZNLB8LXTTMNrVNLBPLiw4lCyCXMM9RKv") },
            new Wallet { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, PublicKey = Encoding.ASCII.GetBytes("SOOHOpGTHmqwnqoM9c0HGWKLGGUIGW8GSOOHImQIGWKGdr4gXtXSHH9MFq") },
            new Wallet { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, PublicKey = Encoding.ASCII.GetBytes("AWWPWxOBPuyevywU9k0POESTOOCQOE8OAWWPQuYQOESOlz4oFbFAPP9UNy") }
        };

        private readonly List<Transaction> transactions = new()
        {
            new Transaction { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, Input = wallets.ElementAt(0), Amount = 5, Output = wallets.ElementAt(1) },
            new Transaction { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, Input = wallets.ElementAt(1), Amount = 10, Output = wallets.ElementAt(2) },
            new Transaction { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, Input = wallets.ElementAt(2), Amount = 200, Output = wallets.ElementAt(0) }
        };


        public IEnumerable<Transaction> GetTransactions()
        {
            return transactions;
        }

        public Transaction GetTransaction(Guid id)
        {
            return transactions.Where(transaction => transaction.Id == id).SingleOrDefault();
        }

        void ITransactionsRepository.CreateTransaction(Transaction transaction)
        {
            transactions.Add(transaction);
        }

        void ITransactionsRepository.UpdateTransaction(Transaction transaction)
        {
            var index = transactions.FindIndex(existingTransaction => existingTransaction.Id == transaction.Id);
            transactions[index] = transaction;
        }

        void ITransactionsRepository.DeleteTransaction(Guid id)
        {
            var index = transactions.FindIndex(existingTransaction => existingTransaction.Id == id);
            transactions.RemoveAt(index);
        }
    }
}
