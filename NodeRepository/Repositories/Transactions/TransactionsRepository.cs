using NodeRepository.Entities;
using System.Text;
using Core;

namespace NodeRepository.Repositories.Transactions
{
    public class TransactionsRepository : ITransactionsRepository
    {

        private static readonly List<ECDsaKey> keys = new()
        {
            new ECDsaKey { },
            new ECDsaKey { },
            new ECDsaKey { }
        };

        private static readonly List<Transaction> transactions = new()
        {
            new Transaction
            (
                id: Guid.NewGuid(),
                creationTime: DateTimeOffset.UtcNow,
                merkleHash: Encoding.ASCII.GetBytes("TUlJQklqQU5CZ2txaGtpRzl3MEJBUUVGQUFPQ0FROEFNSUlCQ2dLQ0FRRUF4bDRhUm5STUJCOUdaaw=="),
                input: keys.ElementAt(0).GetPublicKey(),
                amount: 5,
                output: keys.ElementAt(1).GetPublicKey(),
                isDelegating: true,
                signature: Encoding.ASCII.GetBytes("t4or5p62SBIIvb6hKNxl/6pXt+7wsRwLQTUeq0O1Unmzu6XGWo+oI8g7QAECFY2DxkVlfmYus9Rc79MgV9XvGg==")
            ),
            new Transaction
            (
                id: Guid.NewGuid(),
                creationTime: DateTimeOffset.UtcNow,
                merkleHash: Encoding.ASCII.GetBytes("h+7pHz6TFIP9Qw4zLMnrXOLjFyhYnQDd1KkFyW84XpxMHqbAH4Qv7CQTrGDKN1Xf5vO4ZeBu1iyrpAgKuZP6bQ=="),
                input: keys.ElementAt(2).GetPublicKey(),
                amount: 10,
                output: keys.ElementAt(1).GetPublicKey(),
                isDelegating: false,
                signature: Encoding.ASCII.GetBytes("TUlJQklqQU5CZ2txaGtpRzl3MEJBUUVGQUFPQ0FROEFNSUlCQ2dLQ0FRRUF4bDRhUm5STUJCOUdaaw==")
            ),
            new Transaction
            (
                id: Guid.NewGuid(),
                creationTime: DateTimeOffset.UtcNow,
                merkleHash: Encoding.ASCII.GetBytes("t4or5p62SBIIvb6hKNxl/6pXt+7wsRwLQTUeq0O1Unmzu6XGWo+oI8g7QAECFY2DxkVlfmYus9Rc79MgV9XvGg=="),
                input: keys.ElementAt(2).GetPublicKey(),
                amount: 200,
                output: keys.ElementAt(0).GetPublicKey(),
                isDelegating: true,
                signature: Encoding.ASCII.GetBytes("h+7pHz6TFIP9Qw4zLMnrXOLjFyhYnQDd1KkFyW84XpxMHqbAH4Qv7CQTrGDKN1Xf5vO4ZeBu1iyrpAgKuZP6bQ==")
            )
        };

        public Transaction GetTransaction(Guid id)
        {
            return transactions.Where(transaction => transaction.Id == id).SingleOrDefault();
        }

        public void CreateTransaction(Transaction transaction)
        {
            transactions.Add(transaction);
        }

        public void UpdateTransaction(Transaction transaction)
        {
            var index = transactions.FindIndex(existingTransaction => existingTransaction.Id == transaction.Id);
            transactions[index] = transaction;
        }

        public void DeleteTransaction(Guid id)
        {
            var index = transactions.FindIndex(existingTransaction => existingTransaction.Id == id);
            transactions.RemoveAt(index);
        }

        public IEnumerable<Transaction> GetTransactions(byte[] publicKey)
        {
            return transactions.Where(transaction => transaction.Input == publicKey);
        }

        public Transaction GetTransactionTesting()
        {
            ECDsaKey key = new();
            Transaction transaction = new
            (
                id: Guid.NewGuid(),
                creationTime: DateTimeOffset.UtcNow,
                merkleHash: Encoding.ASCII.GetBytes("t4or5p62SBIIvb6hKNxl/6pXt+7wsRwLQTUeq0O1Unmzu6XGWo+oI8g7QAECFY2DxkVlfmYus9Rc79MgV9XvGg=="),
                input: key.GetPublicKey(),
                amount: 200,
                output: keys.ElementAt(1).GetPublicKey(),
                isDelegating: false
            );
            transaction.Sign(key);
            return transaction;
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return transactions;
        }
    }
}
