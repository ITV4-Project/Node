using NodeRepository.Entities;

namespace NodeRepository.Repositories.Transactions;

public interface ITransactionsRepository
{
    Transaction GetTransaction(Guid id);
    IEnumerable<Transaction> GetTransactions();
    void CreateTransaction(Transaction transaction);
    void UpdateTransaction(Transaction transaction);
    void DeleteTransaction(Guid id);
}