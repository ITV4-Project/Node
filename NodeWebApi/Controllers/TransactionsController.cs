﻿using Microsoft.AspNetCore.Mvc;
using NodeWebApi.Dtos.Transactions;
using NodeWebApi.Entities;
using NodeWebApi.Repositories;
using NodeWebApi.Repositories.Transactions;

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
            var transaction = repository.GetTransactions(id);

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
                CreationDate = DateTimeOffset.UtcNow,
                PublicKey = transactionDto.PublicKey
            };

            repository.CreateTransactions(transaction);

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
                PublicKey = transactionDto.PublicKey
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