using Microsoft.AspNetCore.Mvc;
using NodeWebApi.Dtos.Transactions;
using NodeWebApi.Entities;
using NodeWebApi.Repositories.Transactions;
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
        public IEnumerable<TransactionDto> GetTransactions()
        {
            var transaction = repository.GetTransactions().Select(transaction => transaction.AsDto());
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
            Transaction transaction = new()
            {
                Id = Guid.NewGuid(),
                Version = transactionDto.Version,
                CreationDate = DateTimeOffset.UtcNow,
                Name = transactionDto.Name,
                MerkleHash = transactionDto.MerkleHash,
                Input = transactionDto.Input,
                Amount = transactionDto.Amount,
                Output = transactionDto.Output,
                Delegate = transactionDto.Delegate,
                Signature = transactionDto.Signature
            };


            // Opmaak van data benodigd voor het signen
            byte[] data = repository.SignatureDataConvertToBytes(transaction);

            // Controleren of signature overeenkomt
            // Public key van verzender wordt gebruikt om te controleren of de data en de signature te verifieren
            ECDsaKey ecdsKey = new ECDsaKey(transaction.Input, false);
            if (ecdsKey.Verify(data, transaction.Signature))
                repository.CreateTransaction(transaction);
            else
                return BadRequest();

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction.AsDto());
        }

        // PUT /transactions/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateTransaction(Guid id, UpdateTransactionDto transactionDto)
        {
            var existingTransaction = repository.GetTransaction(id);

            if (existingTransaction == null)
            {
                return NotFound();
            }

            Transaction updatedTransaction = existingTransaction with
            {
                Version = transactionDto.Version,
                Name = transactionDto.Name,
                MerkleHash = transactionDto.MerkleHash,
                Input = transactionDto.Input,
                Amount = transactionDto.Amount,
                Output = transactionDto.Output,
                Delegate = transactionDto.Delegate,
                Signature = transactionDto.Signature
            };

            repository.UpdateTransaction(updatedTransaction);

            return NoContent();
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
    }
}
