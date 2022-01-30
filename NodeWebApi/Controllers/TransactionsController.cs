using Core;
using Core.Database;
using Microsoft.AspNetCore.Mvc;
using NodeNetworking.Buffering;
using NodeWebApi.Dtos.Transactions;


namespace NodeWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ILedger ledger;
        private readonly IBuffer<Transaction> transactionBuffer;

        public TransactionsController(ILedger ledger, IBuffer<Transaction> transactionBuffer)
        {
            this.ledger = ledger;
            this.transactionBuffer = transactionBuffer;
        }

        // GET /transactions
        [HttpGet]
        public IEnumerable<TransactionDto> GetAllTransactions()
        {
            var transaction = ledger.GetAllTransactions().Select(transaction => transaction.AsDto());
            return transaction;
        }

        // GET /transactions/{signature}
        [HttpGet("{signature}")]
        public ActionResult<TransactionDto> GetTransaction(byte[] signature)
        {
            Transaction transaction = ledger.GetTransaction(signature);

            if (transaction == null)
            {
                return NotFound();
            }
            return transaction.AsDto();
        }

        // POST /transactions
        [HttpPost]
        public ActionResult<TransactionDto> CreateTransaction(CreateTransactionDto transaction)
        {
            if (transaction.VerifySignature()) {
                transactionBuffer.Add(transaction);
            } else { 
                return Conflict("Signature invalid.");
            }

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
        public ActionResult<long> GetBalance(string publicKeyHex)
        {
            byte[] publicKey = Convert.FromHexString(publicKeyHex);
            return ledger.GetBalance(publicKey);
        }

        // GET /transactions/{publicKeyHex}
        [HttpGet("/PublicKey/{publicKeyHex}")]
        public ActionResult<IEnumerable<TransactionDto>> GetTransactions(string publicKeyHex)
        {
            byte[] publicKey = Convert.FromHexString(publicKeyHex);

            var transactions = ledger.GetAllTransactions().Where(t => t.Input == publicKey).Select(transaction => transaction.AsDto());

            if (transactions == null)
            {
                return NotFound();
            }
            return transactions.ToList();

        }

        // DELETE /transactions/{id}
        [HttpDelete("{signature}")]
        public ActionResult DeleteTransaction(byte[] signature)
        {
            var existingTransaction = transactionBuffer.GetAllItems().Where(x => x.Signature == signature).FirstOrDefault();

            if (existingTransaction == null)
            {
                return NotFound();
            }

            transactionBuffer.Remove(existingTransaction);

            return NoContent();
        }

        // GET /transactions/test
        [HttpGet("test")]
        public ActionResult<TransactionDto> GetTransactionTesting()
        {
            return new TransactionDto() {
                MerkleHash = Utility.GetEmptyByteArray(128),
                Input = Utility.ConcatArrays(new byte[] { 0x04 }, Utility.GetEmptyByteArray(128)),
                Output = Utility.ConcatArrays(new byte[] { 0x04 }, Utility.GetEmptyByteArray(128)),
                Amount = 100,
                IsDelegating = false
            };
        }
    }
}
