using Microsoft.AspNetCore.Mvc;
using NodeWebApi.Dtos.Transactions;
using NodeRepository.Entities;
using NodeRepository.Repositories.Transactions;
using Core;

namespace NodeWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsRepository repository;

        public TransactionsController(ITransactionsRepository repository)
        {
            this.repository = repository;
        }

        // GET /transactions
        [HttpGet]
        public IEnumerable<TransactionDto> GetAllTransactions()
        {
            var transaction = repository.GetAllTransactions().Select(transaction => transaction.AsDto());
            return transaction;
        }

        // GET /transactions/{id}
        [HttpGet("{id}")]
        public ActionResult<TransactionDto> GetTransaction(Guid id)
        {
            var transaction = repository.GetTransaction(id);

            if (transaction == null)
            {
                return NotFound();
            }
            return transaction.AsDto();
        }

        // POST /transactions
        [HttpPost]
        public ActionResult<TransactionDto> CreateTransaction(CreateTransactionDto transactionDto)
        {
            Transaction transaction = new(
                Guid.NewGuid(), 
                transactionDto.Version,
                transactionDto.CreationTime, 
                transactionDto.MerkleHash, 
                transactionDto.Input, 
                transactionDto.Output, 
                transactionDto.Amount, 
                transactionDto.IsDelegating, 
                transactionDto.Signature
                );

            //// Opmaak van data benodigd voor het signen
            //byte[] data = repository.SignatureDataConvertToBytes(transaction);

            //// Controleren of signature overeenkomt
            //// Public key van verzender wordt gebruikt om te controleren of de data en de signature te verifieren
            //ECDsaKey ecdsKey = new ECDsaKey(transaction.Input, false);
            //if (ecdsKey.Verify(data, transaction.Signature))
            //    repository.CreateTransaction(transaction);

            if (transaction.VerifySignature())
                repository.CreateTransaction(transaction);
            else
                return Conflict("Signature invalid.");

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction.AsDto());
        }

        //// PUT /transactions/{id}
        //[HttpPut("{id}")]
        //public ActionResult UpdateTransaction(Guid id, UpdateTransactionDto transactionDto)
        //{
        //    var existingTransaction = repository.GetTransaction(id);

        //    if (existingTransaction == null)
        //    {
        //        return NotFound();
        //    }

        //    Transaction updatedTransaction = existingTransaction with(
        //        Guid.NewGuid(), 
        //        transactionDto.Version, 
        //        DateTimeOffset.UtcNow, 
        //        transactionDto.MerkleHash, 
        //        transactionDto.Input, 
        //        transactionDto.Output, 
        //        transactionDto.Amount, 
        //        transactionDto.IsDelegating, 
        //        transactionDto.Signature

        //    repository.UpdateTransaction(updatedTransaction);

        //    return NoContent();
        //}

        //GET /balance/{publicKeyHex}
        [HttpGet("/Balance/{publicKeyHex}")]
        public ActionResult<int> GetBalance(string publicKeyHex)
        {
            byte[] publicKey = Convert.FromHexString(publicKeyHex);
            var transactions = repository.GetTransactions(publicKey);

            if (transactions == null)
            {
                return NotFound();
            }

            int balance = 0;

            var outgoing = transactions.Where(transaction => transaction.Input == publicKey);

            var incoming = transactions.Where(transaction => transaction.Output == publicKey);

            foreach (var transaction in outgoing)
                balance -= transaction.Amount;

            foreach (var transaction in incoming)
                balance += transaction.Amount;

            return balance;
        }

        // GET /transactions/{publicKeyHex}
        [HttpGet("/PublicKey/{publicKeyHex}")]
        public ActionResult<IEnumerable<TransactionDto>> GetTransactions(string publicKeyHex)
        {
            byte[] publicKey = Convert.FromHexString(publicKeyHex);

            var transactions = repository.GetTransactions(publicKey).Select(transaction => transaction.AsDto());

            if (transactions == null)
            {
                return NotFound();
            }
            return transactions.ToList();

        }

        // DELETE /transactions/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteTransaction(Guid id)
        {
            var existingTransaction = repository.GetTransaction(id);

            if (existingTransaction == null)
            {
                return NotFound();
            }

            repository.DeleteTransaction(id);

            return NoContent();
        }

        // GET /transactions/test
        [HttpGet("test")]
        public ActionResult<TransactionDto> GetTransactionTesting()
        {
            return repository.GetTransactionTesting().AsDto();
        }
    }
}
