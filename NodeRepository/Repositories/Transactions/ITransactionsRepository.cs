using Core;

namespace NodeRepository.Repositories.Transactions;

public interface ITransactionsRepository
{
    Transaction GetTransaction(Guid id);
    IEnumerable<Transaction> GetAllTransactions();
    void CreateTransaction(Transaction transaction);
    void UpdateTransaction(Transaction transaction);
    void DeleteTransaction(Guid id);
    IEnumerable<Transaction> GetTransactions(byte[] publicKey);
    Transaction GetTransactionTesting();
}