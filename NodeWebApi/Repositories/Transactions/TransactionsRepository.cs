using NodeWebApi.Entities;
using NodeWebApi.Repositories.Wallets;
using System.Security.Cryptography;
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
            new Transaction { Id = Guid.NewGuid(), Version = 1, CreationDate = DateTimeOffset.UtcNow, Name = "John Doe", MerkleHash = Encoding.ASCII.GetBytes("IEEXEfWJXcgmdgeC9s0XWMABWWKYWM8WIEEXYcGYWMAWth4wNjNIXX9CVg"), Input = wallets.ElementAt(0).PublicKey, Amount = 5, Output = wallets.ElementAt(1).PublicKey, Delegate = true, Signature = Encoding.ASCII.GetBytes("OKKDKlCPDimsjmkI9y0DCSGHCCQECS8COKKDEiMECSGCzn4cTpTODD9IBm") },
            new Transaction { Id = Guid.NewGuid(), Version = 1, CreationDate = DateTimeOffset.UtcNow, Name = "Jane Doe", MerkleHash = Encoding.ASCII.GetBytes("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxl4aRnRMBB9GZk"), Input = wallets.ElementAt(1).PublicKey, Amount = 10, Output = wallets.ElementAt(2).PublicKey, Delegate = false, Signature = Encoding.ASCII.GetBytes("PLLELmDQEjntknlJ9z0EDTHIDDRFDT8DPLLEFjNFDTHDao4dUqUPEE9JCn") },
            new Transaction { Id = Guid.NewGuid(), Version = 1, CreationDate = DateTimeOffset.UtcNow, MerkleHash = Encoding.ASCII.GetBytes("NJJCJkBOChlriljH9x0CBRFGBBPDBR8BNJJCDhLDBRFBym4bSoSNCC9HAl"), Input = wallets.ElementAt(2).PublicKey, Amount = 200, Output = wallets.ElementAt(0).PublicKey, Delegate = true, Signature = Encoding.ASCII.GetBytes("QMMFMnERFkoulomK9a0FEUIJEESGEU8EQMMFGkOGEUIEbp4eVrVQFF9KDo") }
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
