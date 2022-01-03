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
            { 
                Id = Guid.NewGuid(), 
                Version = 1, 
                CreationDate = DateTimeOffset.UtcNow, 
                Name = "John Doe", 
                MerkleHash = Encoding.ASCII.GetBytes("TUlJQklqQU5CZ2txaGtpRzl3MEJBUUVGQUFPQ0FROEFNSUlCQ2dLQ0FRRUF4bDRhUm5STUJCOUdaaw=="), 
                Input = keys.ElementAt(0).GetPublicKeyBytes(), 
                Amount = 5, 
                Output = keys.ElementAt(1).GetPublicKeyBytes(), 
                Delegate = true, 
                Signature = Encoding.ASCII.GetBytes("t4or5p62SBIIvb6hKNxl/6pXt+7wsRwLQTUeq0O1Unmzu6XGWo+oI8g7QAECFY2DxkVlfmYus9Rc79MgV9XvGg==")
            },
            new Transaction 
            { 
                Id = Guid.NewGuid(), 
                Version = 1, 
                CreationDate = DateTimeOffset.UtcNow, 
                Name = "Jane Doe", 
                MerkleHash = Encoding.ASCII.GetBytes("h+7pHz6TFIP9Qw4zLMnrXOLjFyhYnQDd1KkFyW84XpxMHqbAH4Qv7CQTrGDKN1Xf5vO4ZeBu1iyrpAgKuZP6bQ=="), 
                Input = keys.ElementAt(1).GetPublicKeyBytes(), 
                Amount = 10, 
                Output = keys.ElementAt(2).GetPublicKeyBytes(), 
                Delegate = false, 
                Signature = Encoding.ASCII.GetBytes("TUlJQklqQU5CZ2txaGtpRzl3MEJBUUVGQUFPQ0FROEFNSUlCQ2dLQ0FRRUF4bDRhUm5STUJCOUdaaw==")
            },
            new Transaction 
            { 
                Id = Guid.NewGuid(), 
                Version = 1, 
                CreationDate = DateTimeOffset.UtcNow, 
                MerkleHash = Encoding.ASCII.GetBytes("t4or5p62SBIIvb6hKNxl/6pXt+7wsRwLQTUeq0O1Unmzu6XGWo+oI8g7QAECFY2DxkVlfmYus9Rc79MgV9XvGg=="), 
                Input = keys.ElementAt(2).GetPublicKeyBytes(), 
                Amount = 200, 
                Output = keys.ElementAt(0).GetPublicKeyBytes(), 
                Delegate = true, 
                Signature = Encoding.ASCII.GetBytes("h+7pHz6TFIP9Qw4zLMnrXOLjFyhYnQDd1KkFyW84XpxMHqbAH4Qv7CQTrGDKN1Xf5vO4ZeBu1iyrpAgKuZP6bQ==") }
        };


        public IEnumerable<Transaction> GetTransactions()
        {
            return transactions;
        }

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

        public Transaction GetTransactionTesting()
        {
            ECDsaKey key = new ECDsaKey();
            Transaction transactionToSign = new Transaction
            {
                Version = 1,
                MerkleHash = Encoding.ASCII.GetBytes("t4or5p62SBIIvb6hKNxl/6pXt+7wsRwLQTUeq0O1Unmzu6XGWo+oI8g7QAECFY2DxkVlfmYus9Rc79MgV9XvGg=="),
                Output = keys.ElementAt(1).GetPublicKeyBytes(),
                Amount = 200,
                Delegate = false
            };

            Transaction transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Version = 1,
                CreationDate = DateTimeOffset.UtcNow,
                Name = "Testing",
                MerkleHash = Encoding.ASCII.GetBytes("t4or5p62SBIIvb6hKNxl/6pXt+7wsRwLQTUeq0O1Unmzu6XGWo+oI8g7QAECFY2DxkVlfmYus9Rc79MgV9XvGg=="),
                Input = key.GetPublicKeyBytes(),
                Amount = 200,
                Output = keys.ElementAt(1).GetPublicKeyBytes(),
                Delegate = false,
                Signature = key.Sign(SignatureDataConvertToBytes(transactionToSign))
            };

            return transaction;
        }

        public byte[] SignatureDataConvertToBytes(Transaction transaction)
        {
            byte[] vb = BitConverter.GetBytes(transaction.Version);
            byte[] mhb = transaction.MerkleHash;
            byte[] ob = transaction.Output;
            byte[] ab = BitConverter.GetBytes(transaction.Amount);
            byte[] db = BitConverter.GetBytes(transaction.Delegate);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(vb);
                Array.Reverse(ab);
                Array.Reverse(db);
            }

            return vb.Concat(vb).Concat(mhb).Concat(ob).Concat(ab).Concat(db).ToArray();
        }
    }
}
