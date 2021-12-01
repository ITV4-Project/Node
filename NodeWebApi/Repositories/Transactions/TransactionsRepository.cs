using NodeWebApi.Entities;
using System.Text;

namespace NodeWebApi.Repositories.Transactions
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly List<Wallet> wallets = new()
        {
            new Wallet { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, PublicKey = Encoding.ASCII.GetBytes("XTTMTuLYMrvbsvtR9h0MLBPQLLZNLB8LXTTMNrVNLBPLiw4lCyCXMM9RKv") },
            new Wallet { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, PublicKey = Encoding.ASCII.GetBytes("SOOHOpGTHmqwnqoM9c0HGWKLGGUIGW8GSOOHImQIGWKGdr4gXtXSHH9MFq") },
            new Wallet { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, PublicKey = Encoding.ASCII.GetBytes("AWWPWxOBPuyevywU9k0POESTOOCQOE8OAWWPQuYQOESOlz4oFbFAPP9UNy") }
        };

        private readonly List<Transaction> transactions = new()
        {
            //new Transaction { Id = Guid.NewGuid(), CreationDate = DateTimeOffset.UtcNow, Inputs = List<Wallet> }
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
            throw new NotImplementedException();
        }

        void ITransactionsRepository.UpdateTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        void ITransactionsRepository.DeleteTransaction(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
}
